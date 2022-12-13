﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AnimFlex.Editor
{
    public class AFStyles
    {
        public class AlignedEditorStyles : IDisposable
        {
            public AlignedEditorStyles()
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return;
                EditorStyles.textField.alignment = AFEditorSettings.Instance.labelAlignment;
                EditorStyles.numberField.alignment = AFEditorSettings.Instance.labelAlignment;
            }
            public void Dispose()
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return;
                EditorStyles.textField.alignment = TextAnchor.UpperLeft;
                EditorStyles.numberField.alignment = TextAnchor.UpperLeft;
            }
        }

        public class EditorLabelWidth : IDisposable
        {
            private float width;
            public EditorLabelWidth(float width = 10)
            {
                this.width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = width;
            }

            public void Dispose()
            {
                EditorGUIUtility.labelWidth = width;
            }
        }
        public class EditorFieldMinWidth : IDisposable
        {
            private float oldWidth;
            public EditorFieldMinWidth(Rect pos, float width = 10)
            {
                this.oldWidth = EditorGUIUtility.labelWidth;
                if (pos.width - EditorGUIUtility.labelWidth < width)
                    EditorGUIUtility.labelWidth = pos.width - width;
            }

            public void Dispose()
            {
                EditorGUIUtility.labelWidth = oldWidth;
            }
        }

        public class GuiColor : IDisposable
        {
            private Color oldCol;
            public GuiColor(Color color)
            {
                oldCol = GUI.color;
                GUI.color = color;
            }
            public void Dispose() => GUI.color = oldCol;
        }

        public class GuiBackgroundColor : IDisposable
        {
            private Color oldCol;
            public GuiBackgroundColor(Color color)
            {
                oldCol = GUI.backgroundColor;
                GUI.backgroundColor = color;
            }
            public void Dispose() => GUI.backgroundColor = oldCol;
        }

        public class GuiForceActive : IDisposable
        {
            private bool wasEnabled;
            public GuiForceActive()
            {
                wasEnabled = GUI.enabled;
                GUI.enabled = true;
            }
            public void Dispose() => GUI.enabled = wasEnabled;
        }

        public class StyledGuiScope : IDisposable
        {
            private GUIStyle labelStyle;
            private GUIStyle largeLabelStyle;
            private GUIStyle popupStyle;
            private UnityEditor.Editor _editor;

            /// <summary>
            /// if editor is null, it won't automatically repaint
            /// </summary>
            public StyledGuiScope(UnityEditor.Editor editor = null) {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return;
                _editor = editor;
                labelStyle = new GUIStyle(EditorStyles.label);
                largeLabelStyle = new GUIStyle(EditorStyles.largeLabel);
                popupStyle = new GUIStyle(EditorStyles.popup);

                EditorStyles.label.font = AFEditorSettings.Instance.font;
                EditorStyles.label.fontSize = AFEditorSettings.Instance.fontSize;
                EditorStyles.label.alignment = AFEditorSettings.Instance.labelAlignment;
                EditorStyles.label.normal.textColor = AFEditorSettings.Instance.labelCol;
                EditorStyles.label.hover.textColor = AFEditorSettings.Instance.labelCol_Hover;
                EditorStyles.label.onHover.textColor = AFEditorSettings.Instance.labelCol_Hover;

                EditorStyles.largeLabel.font = AFEditorSettings.Instance.font;
                EditorStyles.largeLabel.fontSize = AFEditorSettings.Instance.bigFontSize;
                EditorStyles.largeLabel.alignment = AFEditorSettings.Instance.labelAlignment;
                EditorStyles.largeLabel.normal.textColor = AFEditorSettings.Instance.labelCol;

                EditorStyles.popup.font = AFEditorSettings.Instance.font;
                EditorStyles.popup.fontSize = AFEditorSettings.Instance.fontSize;
                EditorStyles.popup.alignment = AFEditorSettings.Instance.labelAlignment;
                EditorStyles.popup.normal.textColor = AFEditorSettings.Instance.popupCol;
            }

            public void Dispose()
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return;
                EditorStyles.label.font = labelStyle.font;
                EditorStyles.label.fontSize = labelStyle.fontSize;
                EditorStyles.label.alignment = labelStyle.alignment;
                EditorStyles.label.normal.textColor = labelStyle.normal.textColor;
                EditorStyles.label.hover.textColor = labelStyle.hover.textColor;
                EditorStyles.label.onHover.textColor = labelStyle.onHover.textColor;

                EditorStyles.largeLabel.font = largeLabelStyle.font;
                EditorStyles.largeLabel.fontSize = largeLabelStyle.fontSize;
                EditorStyles.largeLabel.alignment = largeLabelStyle.alignment;
                EditorStyles.largeLabel.normal.textColor = largeLabelStyle.normal.textColor;

                EditorStyles.popup.font = popupStyle.font;
                EditorStyles.popup.fontSize = popupStyle.fontSize;
                EditorStyles.popup.alignment = popupStyle.alignment;
                EditorStyles.popup.normal.textColor = popupStyle.normal.textColor;

                if ( _editor != null && AFEditorSettings.Instance.repaintEveryFrame ) {
                    _editor.Repaint();
                }
            }

        }

        public static void Refresh() {
            // set all styles to null for refresh
            var styleFIs = typeof(AFStyles)
                .GetFields( BindingFlags.NonPublic | BindingFlags.Static )
                .Where( t => t.Name.StartsWith( "_" ) && t.FieldType == typeof(GUIStyle) );
            foreach (var fieldInfo in styleFIs) {
                fieldInfo.SetValue( null, null );
            }
        }

        private static GUIStyle _button;
        public static GUIStyle Button
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return GUI.skin.button;
                if (_button != null) return _button;
                _button = new GUIStyle(GUI.skin.button);
                _button.normal.textColor = AFEditorSettings.Instance.buttonDefCol;
                _button.font = AFEditorSettings.Instance.font;
                _button.fontSize = AFEditorSettings.Instance.fontSize;
                return _button;
            }
        }

        private static GUIStyle _bigButton;
        public static GUIStyle BigButton
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return Button;
                if (_bigButton != null) return _bigButton;
                _bigButton = new GUIStyle(Button);
                _bigButton.fontSize = AFEditorSettings.Instance.bigFontSize;
                _bigButton.richText = true;
                return _bigButton;
            }
        }

        private static GUIStyle _clearButton;
        public static GUIStyle ClearButton
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return GUI.skin.button;
                if (_clearButton != null) return _clearButton;
                _clearButton = new GUIStyle(Button);
                _clearButton.fontSize = AFEditorSettings.Instance.bigFontSize;
                _clearButton.normal.textColor = AFEditorSettings.Instance.buttonDefCol;
                _clearButton.alignment = TextAnchor.MiddleCenter;

                var tex = new Texture2D(2, 2);
                tex.SetPixels(new[]
                {
                    Color.clear, Color.clear,
                    Color.clear, Color.clear
                });
                tex.Apply(false);
                _clearButton.normal.background = _clearButton.hover.background =
                    _clearButton.onHover.background = tex;
                return _clearButton;
            }
        }

        private static GUIStyle _specialLabel;
        public static GUIStyle SpecialLabel
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return GUI.skin.label;
                if (_specialLabel != null) return _specialLabel;
                _specialLabel = new GUIStyle(GUI.skin.label);
                _specialLabel.font = AFEditorSettings.Instance.font;
                _specialLabel.alignment = AFEditorSettings.Instance.labelAlignment;
                _specialLabel.fontSize = AFEditorSettings.Instance.fontSize;
                _specialLabel.normal.textColor = AFEditorSettings.Instance.labelCol;
                _specialLabel.hover.textColor = _specialLabel.onHover.textColor = AFEditorSettings.Instance.labelCol_Hover;
                return _specialLabel;
            }
        }

        private static GUIStyle _label;
        public static GUIStyle Label
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return GUI.skin.label;
                if (_label != null) return _label;
                _label = new GUIStyle(GUI.skin.label);
                _label.font = AFEditorSettings.Instance.font;
                _label.alignment = AFEditorSettings.Instance.labelAlignment;
                _label.fontSize = AFEditorSettings.Instance.fontSize;
                return _label;
            }
        }

        private static GUIStyle _bigTextField;
        public static GUIStyle BigTextField
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return EditorStyles.textField;
                if (_bigTextField != null) return _bigTextField;
                _bigTextField = new GUIStyle(EditorStyles.textField);
                _bigTextField.font = AFEditorSettings.Instance.font;
                // _bigTextField.fontStyle = FontStyle.Bold;
                _bigTextField.alignment = AFEditorSettings.Instance.labelAlignment;
                _bigTextField.fontSize = AFEditorSettings.Instance.bigFontSize;
                _bigTextField.fixedHeight = 0;
                return _bigTextField;
            }
        }

        private static GUIStyle _popup;
        public static GUIStyle Popup
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return EditorStyles.popup;
                if (_popup != null) return _popup;
                _popup = new GUIStyle(EditorStyles.popup);
                _popup.font = AFEditorSettings.Instance.font;
                _popup.fontSize = AFEditorSettings.Instance.fontSize;
                _popup.stretchHeight = true;
                _popup.stretchWidth = true;
                _popup.alignment = TextAnchor.MiddleCenter;
                _popup.fixedHeight = 0;
                _popup.normal.textColor = _popup.hover.textColor = _popup.active.textColor =
                    _popup.focused.textColor = AFEditorSettings.Instance.popupCol;
                return _popup;
            }
        }

        private static GUIStyle _foldout;
        public static GUIStyle Foldout
        {
            get
            {
                if ( !AFEditorSettings.Instance.useSpecialStyle ) return EditorStyles.foldout;
                if (_foldout != null) return _foldout;
                _foldout = new GUIStyle(EditorStyles.foldout);
                _foldout.font = AFEditorSettings.Instance.font;
                _foldout.fontSize = AFEditorSettings.Instance.fontSize;
                _foldout.fixedHeight = 0;
                _foldout.normal.textColor = _foldout.hover.textColor = _foldout.active.textColor =
                    _foldout.focused.textColor = AFEditorSettings.Instance.labelCol;
                return _foldout;
            }
        }


        
        public static bool DrawBooleanEnum(Rect position, string optionTrue, string optionFalse, bool value, string tooltip, out bool result)
        {
            var options = new GUIContent[]
            {
                new GUIContent(optionTrue, tooltip), new GUIContent(optionFalse, tooltip)
            };
            using var check = new EditorGUI.ChangeCheckScope();
            using (new AFStyles.EditorLabelWidth(0))
	            result = EditorGUI.Popup(position, GUIContent.none, value ? 0 : 1, options, AFStyles.Popup) == 0;
            return check.changed;
        }

        public static void DrawBooleanEnum(Rect position, string optionTrue, string optionFalse, SerializedProperty property)
        {
            var options = new GUIContent[]
            {
                new GUIContent(optionTrue, property.tooltip), new GUIContent(optionFalse, property.tooltip)
            };

            using (new AFStyles.EditorLabelWidth(0))
            {
                property.boolValue = EditorGUI.Popup(position, property.boolValue ? 0 : 1, options, AFStyles.Popup) == 0;
            }

        }


        public static void DrawHelpBox(Rect position, string message, MessageType messageType)
        {
            var GetHelpIcon =
                typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Default);
            if (GetHelpIcon != null)
            {
                var texture = (Texture)GetHelpIcon.Invoke(null, new object[] { messageType });
                var guiContent = new GUIContent(message, texture);

                var style = new GUIStyle(EditorStyles.helpBox);
                style.font = AFEditorSettings.Instance.font;
                style.fontSize = AFEditorSettings.Instance.fontSize;
                style.alignment = AFEditorSettings.Instance.labelAlignment;
                style.wordWrap = false;

                using(new GuiColor(Color.yellow))
                    GUI.Label(position, guiContent, style);
            }
        }

        public static float Height => AFEditorSettings.Instance.height;
        public static float BigHeight => AFEditorSettings.Instance.bigHeight;
        public static float VerticalSpace => AFEditorSettings.Instance.verticalSpace;
        public static Color BoxColor => AFEditorSettings.Instance.BoxCol;
        public static Color BoxColorDarker => AFEditorSettings.Instance.BoxColDarker;
    }
}
