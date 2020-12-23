using System;
using MaterialDesignThemes.Wpf;

namespace CharGraph.Infrastructure
{
	public class ThemeSwitcher
	{
		public static void ApplyBase(bool isDark)
		{
			ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
		}

		private static void ModifyTheme(Action<ITheme> modificationAction)
		{
			PaletteHelper paletteHelper = new PaletteHelper();
			ITheme theme = paletteHelper.GetTheme();

			modificationAction?.Invoke(theme);

			paletteHelper.SetTheme(theme);
		}

	}
}
