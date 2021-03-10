#include <Wire.h>
#include <INA226.h>
#include <Adafruit_MCP4725.h>

INA226 ina;
INA226 ina2;
Adafruit_MCP4725 dac;
Adafruit_MCP4725 dac2;
float gate_old = -200;
float drain_old = -200;

//    case INA226_MODE_POWER_DOWN:      //    case INA226_MODE_SHUNT_TRIG:      //    case INA226_MODE_BUS_TRIG:        //    case INA226_MODE_SHUNT_BUS_TRIG:  //    case INA226_MODE_ADC_OFF:         //    case INA226_MODE_SHUNT_CONT:      //    case INA226_MODE_BUS_CONT:        //    case INA226_MODE_SHUNT_BUS_CONT:

//    case INA226_AVERAGES_1:           //    case INA226_AVERAGES_4:           //    case INA226_AVERAGES_16:          //    case INA226_AVERAGES_64:          //    case INA226_AVERAGES_128:         //    case INA226_AVERAGES_256:         //    case INA226_AVERAGES_512:         //    case INA226_AVERAGES_1024:

//    case INA226_BUS_CONV_TIME_140US:  //    case INA226_BUS_CONV_TIME_204US:  //    case INA226_BUS_CONV_TIME_332US:  //    case INA226_BUS_CONV_TIME_588US:  //    case INA226_BUS_CONV_TIME_1100US: //    case INA226_BUS_CONV_TIME_2116US: //    case INA226_BUS_CONV_TIME_4156US: //    case INA226_BUS_CONV_TIME_8244US:

//    case INA226_SHUNT_CONV_TIME_140US:  //    case INA226_SHUNT_CONV_TIME_204US:  //    case INA226_SHUNT_CONV_TIME_332US:  //    case INA226_SHUNT_CONV_TIME_588US:  //    case INA226_SHUNT_CONV_TIME_1100US: //    case INA226_SHUNT_CONV_TIME_2116US: //    case INA226_SHUNT_CONV_TIME_8244US:

int fuse1 = 100, fuse2 = 20, min1 = -1, min2 = -1, max1 = 10, max2 = 5;
void setup()
{
  Serial.begin(115200);
  Serial.setTimeout(20);
  ina.begin(); //drain
  ina2.begin(0x41); //baze
  // Configure INA226
  ina.configure(INA226_AVERAGES_16, INA226_BUS_CONV_TIME_1100US, INA226_SHUNT_CONV_TIME_1100US, INA226_MODE_SHUNT_BUS_TRIG);
  ina2.configure(INA226_AVERAGES_16, INA226_BUS_CONV_TIME_1100US, INA226_SHUNT_CONV_TIME_1100US, INA226_MODE_SHUNT_BUS_TRIG);
  // Calibrate INA226
  ina.calibrate(0.0546, 2);
  ina2.calibrate(0.270, 1);
  //DAC begin
  dac.begin(0x61);
  dac2.begin(0x60);


}

void loop()
{
  if (Serial.available() > 0) {
    String s = Serial.readString();
    if (s.startsWith("init"))
      mereni();

    else if (s.startsWith("Min1"))
    {
      min1 = s.substring(4).toInt();
    }
    else if (s.startsWith("Min2"))
    {
      min2 = s.substring(4).toInt();
    }
    else if (s.startsWith("Max1"))
    {
      max1 = s.substring(4).toInt();
    }
    else if (s.startsWith("Max2"))
    {
      max2 = s.substring(4).toInt();
    }
    else if (s.startsWith("Fuse1"))
    {
      fuse1 = s.substring(5).toInt();
    }
    else if (s.startsWith("Fuse2"))
    {
      fuse2 = s.substring(5).toInt();
    }
    else if (s.startsWith("Vcal"))
    {
      Vcal();
    }

  }
  delay(100);
}
void mereni() {

  for (double i = min2; i <= max2; i += 0.5 ) {
    volt(min1, i);
    delay(20);
    double current2 = ina2.readShuntCurrent(true) * 1000;
    double voltage = ina2.readBusVoltage() - 11.4;
    if (voltage != gate_old) {
      gate_old = voltage;
      Serial.print("new line |");
      Serial.print(current2, 5);
      Serial.println("mA");

      //Serial.print(voltage, 5);
      //Serial.println("V");

      for (double j = min1; j <= max1; j += (double)(max1 - min1) / 25.00) {
        volt(j, i);
        if (j == min1) delay(50);
        delay(20);
        //measure();
        double current1 = ina.readShuntCurrent(true);
        double voltage2 = ina.readBusVoltage() - 11.4;
        if (voltage2 != drain_old) {
          drain_old = voltage2;

          if (alert(current1 * 1000, current2))break;
          Serial.print(voltage2, 2);
          Serial.print("; ");
          Serial.println(current1, 5);

        }
      }
    }
  }
  volt(0, 0);
  Serial.println("Stop");
}

void volt(float voltage1, float voltage2) {
  voltage1 = map(voltage1, -12.00, 24.00, 0, 4095);
  voltage2 = map(voltage2, -12.00, 24.00, 0, 4095);
  dac.setVoltage(voltage1, false);
  dac2.setVoltage(voltage2, false);
}


void Vcal() {
  Serial.println("Vcal Started");
  int val = 1200;
  int val_old;
  double current_old = -1000;
  double tmp;
  while (true) {
    dac.setVoltage(val,false);
    delay(20);
    tmp = ina.readShuntCurrent(true);
    Serial.println(tmp,5);
    if (tmp == 0 || (current_old > 0 && tmp < 0) || (current_old < 0 && tmp > 0)) {
      if (abs(tmp) < abs(current_old)){
          PrintCal(0, val);
      }
      else{
            PrintCal(0, val_old);
      }
      break;
      }
  else if (tmp > 0 && current_old > 0) {
      val_old = val;
      val--;
    }

    else if (tmp < 0 && current_old < 0) {
      val_old = val;
      val++;
    }
    current_old = tmp;
  }

  







  
}


void PrintCal(int napeti, int kroky) {
  Serial.print("Vcal pro V: ");
  Serial.print(napeti);
  Serial.print(" nalezeno na: ");
  Serial.println(kroky);



}
bool alert(float cur1, float cur2)
{
  if (abs(cur1) > fuse1 || abs(cur2) > fuse2) {

    volt(0, 0);
    return true;
  }
  return false;

}
