//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/TestInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Generated
{
    public partial class @TestInput: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @TestInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""TestInput"",
    ""maps"": [
        {
            ""name"": ""Selection"",
            ""id"": ""a9cafb4f-56f9-4da0-a0ab-e7153b90ddeb"",
            ""actions"": [
                {
                    ""name"": ""left"",
                    ""type"": ""Button"",
                    ""id"": ""ebdc10b0-5895-40a9-867d-7d69d5b54719"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""right"",
                    ""type"": ""Button"",
                    ""id"": ""c033c503-eb1a-45f9-a089-bd335f30ea3f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""enter"",
                    ""type"": ""Button"",
                    ""id"": ""240afdc2-60ed-46a4-9655-66d106cbdd31"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""cancel"",
                    ""type"": ""Button"",
                    ""id"": ""a81ef7f9-b45d-4441-9adb-036c6c3e2682"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""61558f33-e135-4776-b533-6b35008a3f43"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a71a3a29-fa6a-4008-bed4-5bbe55bd7c21"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9462b248-fe2d-46cd-8c13-fed37473bf7d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""039ed47a-2bc6-416a-bbc3-f444b5a6a38f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce6e6b5c-1d30-4e4e-b2de-b03c8201f627"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""500729c4-2cd5-4664-b0ae-52d83a2c3dc8"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""enter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""33c716c0-652e-42fa-a013-3118cf9166b9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6b648c5-82d4-4ca6-a8f5-34e827aa4928"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Selection
            m_Selection = asset.FindActionMap("Selection", throwIfNotFound: true);
            m_Selection_left = m_Selection.FindAction("left", throwIfNotFound: true);
            m_Selection_right = m_Selection.FindAction("right", throwIfNotFound: true);
            m_Selection_enter = m_Selection.FindAction("enter", throwIfNotFound: true);
            m_Selection_cancel = m_Selection.FindAction("cancel", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Selection
        private readonly InputActionMap m_Selection;
        private List<ISelectionActions> m_SelectionActionsCallbackInterfaces = new List<ISelectionActions>();
        private readonly InputAction m_Selection_left;
        private readonly InputAction m_Selection_right;
        private readonly InputAction m_Selection_enter;
        private readonly InputAction m_Selection_cancel;
        public struct SelectionActions
        {
            private @TestInput m_Wrapper;
            public SelectionActions(@TestInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @left => m_Wrapper.m_Selection_left;
            public InputAction @right => m_Wrapper.m_Selection_right;
            public InputAction @enter => m_Wrapper.m_Selection_enter;
            public InputAction @cancel => m_Wrapper.m_Selection_cancel;
            public InputActionMap Get() { return m_Wrapper.m_Selection; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(SelectionActions set) { return set.Get(); }
            public void AddCallbacks(ISelectionActions instance)
            {
                if (instance == null || m_Wrapper.m_SelectionActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_SelectionActionsCallbackInterfaces.Add(instance);
                @left.started += instance.OnLeft;
                @left.performed += instance.OnLeft;
                @left.canceled += instance.OnLeft;
                @right.started += instance.OnRight;
                @right.performed += instance.OnRight;
                @right.canceled += instance.OnRight;
                @enter.started += instance.OnEnter;
                @enter.performed += instance.OnEnter;
                @enter.canceled += instance.OnEnter;
                @cancel.started += instance.OnCancel;
                @cancel.performed += instance.OnCancel;
                @cancel.canceled += instance.OnCancel;
            }

            private void UnregisterCallbacks(ISelectionActions instance)
            {
                @left.started -= instance.OnLeft;
                @left.performed -= instance.OnLeft;
                @left.canceled -= instance.OnLeft;
                @right.started -= instance.OnRight;
                @right.performed -= instance.OnRight;
                @right.canceled -= instance.OnRight;
                @enter.started -= instance.OnEnter;
                @enter.performed -= instance.OnEnter;
                @enter.canceled -= instance.OnEnter;
                @cancel.started -= instance.OnCancel;
                @cancel.performed -= instance.OnCancel;
                @cancel.canceled -= instance.OnCancel;
            }

            public void RemoveCallbacks(ISelectionActions instance)
            {
                if (m_Wrapper.m_SelectionActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ISelectionActions instance)
            {
                foreach (var item in m_Wrapper.m_SelectionActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_SelectionActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public SelectionActions @Selection => new SelectionActions(this);
        public interface ISelectionActions
        {
            void OnLeft(InputAction.CallbackContext context);
            void OnRight(InputAction.CallbackContext context);
            void OnEnter(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
        }
    }
}
