using System;
using System.Runtime.InteropServices;
namespace Xbox_Controller_Mouse
{
    public class XController
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray)] INPUT[] pInputs, int cbSize);

        [DllImport("xinput1_4.dll")]
        private static extern int XInputGetState(uint dwUserIndex, out XINPUT_STATE pState);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XINPUT_GAMEPAD
        {
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        private const uint XINPUT_GAMEPAD_A = 0x1000;
        private const uint XINPUT_GAMEPAD_B = 0x2000;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        public int WHEEL_DELTA = 100; // Valeur standard pour un "tick" de la molette de défilement
        private const float DEADZONE = 0.15f; // Zone morte pour les joysticks
        public bool success;
        public int moveStep = 10; // Nombre de pixels à déplacer à chaque mise à jour
        public void Start()
        {
            bool isButtonAPressed = false; // État actuel du bouton "A"
            bool wasButtonAPressed = false; // État précédent du bouton "A"

            bool isButtonBPressed = false; // État actuel du bouton "B"
            bool wasButtonBPressed = false; // État précédent du bouton "B"

            while (true)
            {
                XINPUT_STATE state;
                int result = XInputGetState(0, out state); // 0 pour le premier contrôleur

                if (result != 0)
                {
                    Console.WriteLine("Controller not connected.");
                    success = false;
                }
                else
                {
                    success = true;
                }

                float leftThumbX = state.Gamepad.sThumbLX;
                float leftThumbY = state.Gamepad.sThumbLY;
                float rightThumbY = state.Gamepad.sThumbRY;

                // Appliquer la zone morte
                if (Math.Abs(leftThumbX) < DEADZONE * short.MaxValue)
                    leftThumbX = 0;
                if (Math.Abs(leftThumbY) < DEADZONE * short.MaxValue)
                    leftThumbY = 0;
                if (Math.Abs(rightThumbY) < DEADZONE * short.MaxValue)
                    rightThumbY = 0;

                // Normaliser les valeurs des joysticks
                int deltaX = (int)((leftThumbX / (float)short.MaxValue) * moveStep);
                int deltaY = (int)((leftThumbY / (float)short.MaxValue) * moveStep);

                // Obtenir la position actuelle du curseur
                if (GetCursorPos(out POINT currentPos))
                {
                    // Calculer la nouvelle position du curseur
                    int newX = currentPos.X + deltaX;
                    int newY = currentPos.Y - deltaY;

                    // Vérifier si la nouvelle position est différente de la position actuelle
                    if (newX != currentPos.X || newY != currentPos.Y)
                    {
                        // Déplacer la souris
                        SetCursorPos(newX, newY);
                    }
                }

                // Détecter l'état actuel des boutons
                isButtonAPressed = (state.Gamepad.wButtons & XINPUT_GAMEPAD_A) == XINPUT_GAMEPAD_A;
                isButtonBPressed = (state.Gamepad.wButtons & XINPUT_GAMEPAD_B) == XINPUT_GAMEPAD_B;

                // Gestion du clic gauche (bouton A)
                if (isButtonAPressed && !wasButtonAPressed)
                {
                    // Simuler un clic gauche
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }
                else if (!isButtonAPressed && wasButtonAPressed)
                {
                    // Relâcher le clic gauche
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }

                // Gestion du clic droit (bouton B)
                if (isButtonBPressed && !wasButtonBPressed)
                {
                    // Simuler un clic droit
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                }
                else if (!isButtonBPressed && wasButtonBPressed)
                {
                    // Relâcher le clic droit
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }

                // Simulation du défilement avec le joystick droit
                int scrollAmount = (int)((rightThumbY / (float)short.MaxValue) * WHEEL_DELTA);
                if (scrollAmount != 0)
                {
                    INPUT[] inputs = new INPUT[1];
                    inputs[0].type = 0; // INPUT_MOUSE
                    inputs[0].mi.dx = 0;
                    inputs[0].mi.dy = 0;
                    inputs[0].mi.mouseData = (uint)scrollAmount;
                    inputs[0].mi.dwFlags = MOUSEEVENTF_WHEEL;
                    inputs[0].mi.time = 0;
                    inputs[0].mi.dwExtraInfo = IntPtr.Zero;

                    SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
                }

                // Mettre à jour les états précédents des boutons
                wasButtonAPressed = isButtonAPressed;
                wasButtonBPressed = isButtonBPressed;

                // Attendre un court instant pour éviter une boucle trop rapide
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}