﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.System;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using CommunityToolkit.WinUI.UI;
using static Hi3Helper.Logger;
using static Hi3Helper.Locale;
using static Hi3Helper.Shared.Region.LauncherConfig;
using static CollapseLauncher.Dialogs.SimpleDialogs;

namespace CollapseLauncher.Dialogs
{
    public static class KeyboardShortcuts
    {
        public static event EventHandler<int> KeyboardShortcutsEvent;
        private static string colorSchm = Application.Current.RequestedTheme == ApplicationTheme.Dark ? "SystemAccentColorLight2" : "SystemAccentColorDark1";
        private static int pageNum = 0;
        private static int oldSender = 0;

        #region UI Methods
        public static async Task<ContentDialogResult> Dialog_ShowKbShortcuts(UIElement Content, int page = 0)
        {
            StackPanel mainStack = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(0, 0, 0, -7) };

            StackPanel mainStackContent = new StackPanel() { Orientation = Orientation.Horizontal };

            List<List<string>> keys = KeyList;
            oldSender = page;

            // General shortcuts
            StackPanel genStack = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed };
            genStack.Children.Add(new TextBlock { Text = "General", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 8, 0, 2) });
            genStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 8, 0, 8) });
            genStack.Children.Add(GenerateShortcutBlock(keys[2], "Open this menu", "It can also be accessed through the App Settings"));
            genStack.Children.Add(GenerateShortcutBlock(keys[3], "Go to the Home page"));
            genStack.Children.Add(GenerateShortcutBlock(keys[4], "Go to the Settings page"));
            genStack.Children.Add(GenerateShortcutBlock(keys[5], "Open the Notification Tray"));
            genStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 10, 0, 8) });
            pageNum++;

            // Region/Game Shortcuts
            StackPanel changeStack = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed };
            changeStack.Children.Add(new TextBlock { Text = "Quick Game/Region change", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 16, 0, 2) });
            changeStack.Children.Add(new TextBlock { Text = "Note: The keybinds follow the selector order.", FontSize = 11.5 });

            string gameMod = keys[0][0];
            string regionMod = keys[1][0];

            StackPanel textBlock = new StackPanel() { Orientation = Orientation.Horizontal };
            textBlock.Children.Add(new TextBlock()
            {
                Text = "",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = Application.Current.Resources["FontAwesomeSolid"] as FontFamily,
            });
            textBlock.Children.Add(new TextBlock() { Text = string.Format("Swap {0} and {1}", gameMod, regionMod), Margin = new Thickness(8, 0, 0, 0) });

            Button modifierSwap = new Button()
            {
                Content = textBlock,
                CornerRadius = new CornerRadius(5),
                DataContext = new List<string> { "Ctrl", "1 - X" },
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, -30, 2, 0)
            };
            modifierSwap.Click += Swap_Click;
            changeStack.Children.Add(modifierSwap);

            changeStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 8, 0, 8) });
            changeStack.Children.Add(GenerateShortcutBlock(keys[0], "Change game", string.Format("You can just use the NumPad for this shortcut too!\nE.g. {0}+1 or NumPad1 leads Honkai Impact 3rd's page (last used region)", gameMod), false));
            changeStack.Children.Add(GenerateShortcutBlock(keys[1], "Change region", string.Format("E.g. For Genshin Impact, {0}+1 leads to the Global region", regionMod), false));
            changeStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 10, 0, 8) });
            pageNum++;

            // Game folder
            StackPanel gameFolderStack = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed };
            gameFolderStack.Children.Add(new TextBlock { Text = "Game Folder Access", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 8, 0, 2) });
            gameFolderStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 8, 0, 8) });
            gameFolderStack.Children.Add(GenerateShortcutBlock(keys[6], "Open the Screenshot folder"));
            gameFolderStack.Children.Add(GenerateShortcutBlock(keys[7], "Open the Game folder"));
            gameFolderStack.Children.Add(GenerateShortcutBlock(keys[8], "Open the Cache folder"));
            gameFolderStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 10, 0, 8) });
            pageNum++;

            // Game management
            StackPanel gameManageStack = new StackPanel() { Orientation = Orientation.Vertical, Visibility = Visibility.Collapsed };
            gameManageStack.Children.Add(new TextBlock { Text = "Game Management", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 8, 0, 2) });
            gameManageStack.Children.Add(new TextBlock { Text = "Note: These keybinds only work if such feature is supported in the region", FontSize = 11.5 });
            gameManageStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 8, 0, 8) });
            gameManageStack.Children.Add(GenerateShortcutBlock(keys[9], "Close the game forcefully"));
            gameManageStack.Children.Add(GenerateShortcutBlock(keys[10], "Go to the Repair page"));
            gameManageStack.Children.Add(GenerateShortcutBlock(keys[11], "Go to the Game Settings page"));
            gameManageStack.Children.Add(GenerateShortcutBlock(keys[12], "Go to the Caches page"));
            gameManageStack.Children.Add(new MenuFlyoutSeparator() { Margin = new Thickness(0, 10, 0, 8) });
            pageNum = 0;

            StackPanel buttonStack = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Center, Orientation = Orientation.Horizontal, Margin = new Thickness(0, 7, 0, 0) };

            Button genButton = new Button() { DataContext = 0, Content = new TextBlock() { Text = "General" }, Margin = new Thickness(5, 0, 5, 0) };
            Button changeButton = new Button() { DataContext = 1, Content = new TextBlock() { Text = "Quick Switch" }, Margin = new Thickness(5, 0, 5, 0) };
            Button gameFolderButton = new Button() { DataContext = 2, Content = new TextBlock() { Text = "Folder Access" }, Margin = new Thickness(5, 0, 5, 0) };
            Button gameManagerButton = new Button() { DataContext = 3, Content = new TextBlock() { Text = "Game Management" }, Margin = new Thickness(5, 0, 5, 0) };

            List<object> stacks = new List<object>() { genStack, changeStack, gameFolderStack, gameManageStack };
            List<object> buttons = new List<object>() { genButton, changeButton, gameFolderButton, gameManagerButton };

            foreach (Button button in buttons)
            {
                button.Click += (o, e) => { ChangeMenuVisibility((int)((Button)o).DataContext, stacks, buttons); };
                buttonStack.Children.Add(button);
            }

            mainStack.Children.Add(mainStackContent);
            mainStack.Children.Add(buttonStack);

            foreach (StackPanel stk in stacks)
            {
                stk.Width = 500;
                mainStackContent.Children.Add(stk);
            }
            ChangeMenuVisibility(page, stacks, buttons);

            return await SpawnDialog(
                    "Keyboard Shortcuts",
                    mainStack,
                    Content,
                    Lang._Misc.Close,
                    null,
                    null,
                    ContentDialogButton.Primary
                );
        }

        private static Grid GenerateShortcutBlock(List<string> kbKeys, string description, string example = null, bool enableSwapButton = true)
        {
            Grid shortcut = new Grid()
            {
                Margin = new Thickness(0, 8, 0, 8),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ColumnSpacing = 5,
                ColumnDefinitions = { new ColumnDefinition() { Width = GridLength.Auto }, new ColumnDefinition() }
            };

            StackPanel shortcutDesc = new StackPanel()
            {
                Margin = new Thickness(0, 1, 5, 1),
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center
            };

            int MaxLen = enableSwapButton ? 320 : 370; 

            if (example != null)
            {
                shortcutDesc.Children.Add(new TextBlock() { Text = description, FontSize = 14, Margin = new Thickness(5, 2, 0, 1), TextWrapping = TextWrapping.Wrap, MaxWidth = MaxLen });
                shortcutDesc.Children.Add(new TextBlock() { Text = example, FontSize = 11, Margin = new Thickness(5, 1, 0, 2), TextWrapping = TextWrapping.Wrap, MaxWidth = MaxLen });
            }
            else
            {
                shortcutDesc.Children.Add(new TextBlock() { Text = description, FontSize = 14, Margin = new Thickness(5, 1, 5, 1), TextWrapping = TextWrapping.Wrap, MaxWidth = MaxLen });
            }

            shortcut.Children.Add(shortcutDesc);
            Grid.SetColumn(shortcutDesc, 0);

            StackPanel shortcutButtons = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Orientation = Orientation.Horizontal
            };

            List<string> dataKeys = new List<string> { kbKeys[0], kbKeys[1], description, pageNum.ToString() };
            if (enableSwapButton)
            {
                Button shortcutSwap = new Button()
                {
                    Content = new TextBlock() { Text = "", FontSize = 12, Margin = new Thickness(-5, 0, -5, 0), FontFamily = Application.Current.Resources["FontAwesomeSolid"] as FontFamily },
                    CornerRadius = new CornerRadius(5),
                    DataContext = dataKeys,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                shortcutButtons.Children.Add(shortcutSwap);
                shortcutSwap.Click += Swap_Click;
            }

            foreach (string key in kbKeys)
            {
                shortcutButtons.Children.Add(CreateKeyBoardButton(key));
                shortcutButtons.Children.Add(new TextBlock()
                {
                    Text = "+",
                    FontWeight = FontWeights.Bold,
                    FontSize = 20,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                });
            }

            shortcut.Children.Add(shortcutButtons);
            shortcutButtons.Children.RemoveAt(shortcutButtons.Children.Count - 1);

            Grid.SetColumn(shortcutButtons, 1);

            return shortcut;
        }

        private static Border CreateKeyBoardButton(string key)
        {
            Border keyboxBorder = new Border()
            {
                Height = 42,
                Width = 42,
                Margin = new Thickness(5, 0, 5, 0),
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush((Windows.UI.Color)Application.Current.Resources[colorSchm])
            };

            ThemeShadow ts = new ThemeShadow();
            ts.Receivers.Add(keyboxBorder);
            keyboxBorder.Translation += Shadow48;
            keyboxBorder.Shadow = ts;

            TextBlock keybox = new TextBlock()
            {
                Text = key,
                Foreground = new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark ? Microsoft.UI.Colors.Black : Microsoft.UI.Colors.White),
                FontWeight = FontWeights.Medium,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            keyboxBorder.Child = keybox;

            return keyboxBorder;
        }

        private static readonly Style DefaultStyle = Application.Current.Resources["DefaultButtonStyle"] as Style;
        private static readonly Style AccentStyle = Application.Current.Resources["AccentButtonStyle"] as Style;
        private static async void ChangeMenuVisibility(int sender, List<object> stacks, List<object> buttons)
        {
            try
            {
                (buttons[oldSender] as Button).Style = DefaultStyle;
                StackPanel oldStack = stacks[oldSender] as StackPanel;
                (buttons[sender] as Button).Style = AccentStyle;
                StackPanel newStack = stacks[sender] as StackPanel;

                if (sender == oldSender)
                {
                    oldStack.Visibility = Visibility.Collapsed;
                    newStack.Visibility = Visibility.Visible;
                    return;
                }

                oldSender = sender;

                Storyboard storyboard = new Storyboard();
                DoubleAnimation OpacityAnimation = new DoubleAnimation();
                OpacityAnimation.From = 1;
                OpacityAnimation.To = 0;
                OpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.2));

                Storyboard.SetTarget(OpacityAnimation, oldStack);
                Storyboard.SetTargetProperty(OpacityAnimation, "Opacity");
                storyboard.Children.Add(OpacityAnimation);

                storyboard.Begin();

                await Task.Delay(200);
                newStack.Visibility = Visibility.Visible;
                oldStack.Visibility = Visibility.Collapsed;

                Storyboard storyboard2 = new Storyboard();
                DoubleAnimation OpacityAnimation2 = new DoubleAnimation();
                OpacityAnimation2.From = 0;
                OpacityAnimation2.To = 1;
                OpacityAnimation2.Duration = new Duration(TimeSpan.FromSeconds(0.2));

                Storyboard.SetTarget(OpacityAnimation2, newStack);
                Storyboard.SetTargetProperty(OpacityAnimation2, "Opacity");
                storyboard2.Children.Add(OpacityAnimation2);

                storyboard2.Begin();

            }
            catch (Exception e)
            {
                LogWriteLine(e.ToString());
            }
        }
#endregion

        #region Change Shortcut Methods
        private static async Task Dialog_SwitchKey(UIElement content, List<string> oldKeys)
        {
            StackPanel mainSwitchKeyContent = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Width = 350
            };

            StackPanel HelpStack = new StackPanel() { Orientation = Orientation.Vertical, MaxWidth = 260 };
            Flyout HelpFlyout = new Flyout() { Content = HelpStack, Placement = Microsoft.UI.Xaml.Controls.Primitives.FlyoutPlacementMode.RightEdgeAlignedTop };
            HelpStack.Children.Add(new TextBlock()
            {
                Text = "A shortcut is composed by 2 parts:",
                Margin = new Thickness(0, 2, 0, 4),
                TextWrapping = TextWrapping.Wrap
            });
            HelpStack.Children.Add(new TextBlock()
            {
                Text = "・ Modifier - Shift, Control or Alt/Menu",
                Margin = new Thickness(5, 4, 0, 4)
            });
            HelpStack.Children.Add(new TextBlock()
            {
                Text = "・ Key - Alphabetical (A to Z) or Tab",
                Margin = new Thickness(5, 4, 0, 8)
            });
            HelpStack.Children.Add(new TextBlock()
            {
                Text = "You can choose any combination consisting of one value from each category, unless it is reserved for the system or it is already being used.",
                Margin = new Thickness(0, 4, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });

            StackPanel introPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            introPanel.Children.Add(new TextBlock()
            {
                Text = "Type the new combination for this shortcut!",
                Margin = new Thickness(0, 0, 0, 2),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            Button HelpButton = new Button()
            {
                Content = new TextBlock() { Text = "info", FontFamily = Application.Current.Resources["FontAwesomeSolid"] as FontFamily, FontSize = 10 },
                Flyout = HelpFlyout,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(8, -2, 0, 2)
            };
            introPanel.Children.Add(HelpButton);
            mainSwitchKeyContent.Children.Add(introPanel);

            mainSwitchKeyContent.Children.Add(new TextBlock()
            {
                Text = oldKeys.Last(),
                Margin = new Thickness(5, 6, 0, 10),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold
            });

            StackPanel keysPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 8, 0, -5)
            };

            oldKeys = oldKeys.Take(2).ToList();

            keysPanel.Children.Add(CreateKeyBoardButton(oldKeys[0]));
            keysPanel.Children.Add(new TextBlock() { Text = "+", FontSize = 20, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            keysPanel.Children.Add(CreateKeyBoardButton(oldKeys[1]));
            keysPanel.Children.Add(new FontIcon() { Glyph = "arrow-right", FontSize = 15, FontFamily = Application.Current.Resources["FontAwesomeSolid"] as FontFamily, Margin = new Thickness(10, 0, 10, 0) });
            Border newKey1 = CreateKeyBoardButton("?");
            keysPanel.Children.Add(newKey1);
            keysPanel.Children.Add(new TextBlock() { Text = "+", FontSize = 20, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            Border newKey2 = CreateKeyBoardButton("?");
            keysPanel.Children.Add(newKey2);
            mainSwitchKeyContent.Children.Add(keysPanel);

            ContentDialog result = new ContentDialog
            {
                Title = "Change Shortcut",
                Content = mainSwitchKeyContent,
                CloseButtonText = Lang._Misc.Cancel,
                PrimaryButtonText = Lang._Misc.Change,
                SecondaryButtonText = null,
                IsPrimaryButtonEnabled = false,
                DefaultButton = ContentDialogButton.Primary,
                Background = (Brush)Application.Current.Resources["DialogAcrylicBrush"],
                XamlRoot = content.XamlRoot
            };

            int keyCount = 0;
            TextBlock text1 = newKey1.Child as TextBlock;
            TextBlock text2 = newKey2.Child as TextBlock;
            result.KeyDown += (e, s) =>
            {
                VirtualKey inputKey = s.Key;
                
                keyCount = 0;
                result.IsPrimaryButtonEnabled = false;
                switch (s.Key)
                {
                    case VirtualKey.Control:
                        keyCount++;
                        text1.Text = "Ctrl";
                        text2.Text = "?";
                        break;
                    case VirtualKey.Shift:
                        keyCount++;
                        text1.Text = "Shift";
                        text2.Text = "?";
                        break;
                    case VirtualKey.Menu:
                        keyCount++;
                        text1.Text = "Alt";
                        text2.Text = "?";
                        break;
                }
            };

            List<string> newKeys = null;

            result.KeyUp += (e, s) =>
            {
                int keyValue = (int)s.Key;
                
                if (keyCount >= 1 && ((keyValue >= 0x41 && keyValue <= 0x5A) ||  keyValue == 9)) // Virtual-Key codes for Letters and Tab
                {
                    string keyStr = s.Key.ToString();
                    text2.Text = keyStr;

                    newKeys = new List<string> { text1.Text, text2.Text };
                    result.IsPrimaryButtonEnabled = ValidKeyCombination(newKeys);
                }
            };


            if (await result.ShowAsync() == ContentDialogResult.Primary)
            {
                ChangeKeybind(oldKeys, newKeys);
            }
            return;
        }

        private static async void Swap_Click(object sender, RoutedEventArgs e)
        {
            List<string> keys = (List<string>)(sender as Button).DataContext;

            if (keys[1].StartsWith("1 - "))
            {
                SwapKeybind();
                (sender as Button).FindParent<ContentDialog>().Hide();
                await Task.Delay(200);
                await Dialog_ShowKbShortcuts(sender as UIElement, 1);
            }
            else
            {
                try
                {
                    (sender as Button).FindParent<ContentDialog>().Hide();
                    await Task.Delay(200);
                    await Dialog_SwitchKey(sender as UIElement, keys.SkipLast(1).ToList());
                    await Dialog_ShowKbShortcuts(sender as UIElement, int.Parse(keys.Last()));
                }
                catch (Exception ex)
                {
                    LogWriteLine(ex.ToString());
                }

            }
        }

        private static void ChangeKeybind(List<string> oldKeys, List<string> newKeys)
        {
            try
            {
                List<List<string>> keys = KeyList;
                keys[keys.FindIndex(i => i.SequenceEqual(oldKeys))] = newKeys;

                LogWriteLine($"Swapped {string.Join("+", oldKeys)} with {string.Join("+", newKeys)}");
                KeyList = keys;
                KeyboardShortcutsEvent(null, 1);
            }
            catch (Exception ex)
            {
                LogWriteLine(ex.ToString());
            }
        }

        private static void SwapKeybind()
        {
            try
            {
                List<List<string>> keys = KeyList;

                (keys[1][0], keys[0][0]) = (keys[0][0], keys[1][0]);

                KeyList = keys;
                KeyboardShortcutsEvent(null, 1);
            }
            catch (Exception ex)
            {
                LogWriteLine(ex.ToString());
            }
        }

        public static void ResetKeyboardShortcuts()
        {
            KeyList = null;
        }
        #endregion

        #region Conversion and Validation Methods
        public static VirtualKey StrToVKey(string key)
        {
            //if (key.Contains("Num")) key = "NumberPad" + key[3];
            return (VirtualKey)Enum.Parse(typeof(VirtualKey), key);
        }

        public static VirtualKeyModifiers StrToVKeyModifier(string key)
        {
            switch (key)
            {
                case "Ctrl":
                    key = "Control";
                    break;
                case "Alt":
                    key = "Menu";
                    break;
            }

            return (VirtualKeyModifiers)Enum.Parse(typeof(VirtualKeyModifiers), key);
        }
        private static bool ValidKeyCombination(List<string> keys)
        {
            return !KeyList.Any(i => i.SequenceEqual(keys)) 
                && !forbiddenKeyList.Any(i => i.SequenceEqual(keys));
        }
        #endregion

        #region Key Lists
        private readonly static List<List<string>> defaultKeyList = new List<List<string>>
                {
                    new List<string> { "Ctrl", "1 - 3" },   // Game selection
                    new List<string> { "Shift", "1 - 6" },  // Region selection

                    new List<string> { "Ctrl", "Tab" },     // Keybind menu
                    new List<string> { "Ctrl", "H" },       // Home page
                    new List<string> { "Ctrl", "S" },       // Settings page
                    new List<string> { "Ctrl", "Q" },       // Notification panel

                    new List<string> { "Shift", "X" },      // Screenshot folder
                    new List<string> { "Shift", "F" },      // Game folder
                    new List<string> { "Shift", "G" },      // Cache folder
                    new List<string> { "Shift", "E" },      // Force close game

                    new List<string> { "Shift", "R" },      // Repair page
                    new List<string> { "Shift", "S" },      // Game settings page
                    new List<string> { "Shift", "C" }       // Caches page

                };

        private readonly static List<List<string>> forbiddenKeyList = new List<List<string>>
                {
                    new List<string> { "Ctrl", "A" },
                    new List<string> { "Ctrl", "X" },
                    new List<string> { "Ctrl", "C" },
                    new List<string> { "Ctrl", "V" }
                };

        public static List<List<string>> KeyList
        {
            get
            {
                string keyListStr = GetAppConfigValue("KbShortcutList").ToString();
                List<List<string>> resultList = new List<List<string>>();

                if (keyListStr == null)
                {
                    foreach (List<string> list in defaultKeyList)
                    {
                        resultList.Add(list);
                    }
                    return resultList;
                }

                foreach (string combination in keyListStr.Split('|'))
                {
                    resultList.Add(combination.Split(",").ToList());
                }

                if (resultList.Count < defaultKeyList.Count)
                {
                    resultList.InsertRange(resultList.Count, defaultKeyList.GetRange(resultList.Count, defaultKeyList.Count - resultList.Count));
                }

                return resultList;
            }

            set
            {
                value ??= defaultKeyList;
                string res = "";
                foreach (List<string> key in value)
                {
                    res += string.Join(",", key) + "|";
                }
                res = res.Remove(res.Length - 1);
                LogWriteLine("KeyList was updated to: " + res);
                SetAndSaveConfigValue("KbShortcutList", res);
            }
        }
        #endregion
    }
}