using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Windows.UI.Core;

namespace Files.Extensions
{
    public static class KeyboardAcceleratorExtensions
    {
        public static bool CheckIsPressed(this KeyboardAccelerator keyboardAccelerator)
        {
            return KeyboardInput.GetKeyStateForCurrentThread(keyboardAccelerator.Key).HasFlag(CoreVirtualKeyStates.Down) && // check if the main key is pressed
                (!keyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Menu) || KeyboardInput.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down)) && // check if menu (alt) key is a modifier, and if so check if it's pressed
                (!keyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Shift) || KeyboardInput.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down)) && // check if shift key is a modifier, and if so check if it's pressed
                (!keyboardAccelerator.Modifiers.HasFlag(VirtualKeyModifiers.Control) || KeyboardInput.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down)); // check if ctrl key is a modifier, and if so check if it's pressed
        }
    }
}
