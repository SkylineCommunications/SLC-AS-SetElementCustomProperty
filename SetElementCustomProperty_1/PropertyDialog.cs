using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetElementCustomProperty_1
{
    using System;
    using System.Collections.ObjectModel;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class PropertyDialog : Dialog
    {
        private readonly Label propertyValueLabel = new Label();
        private readonly Label warningLabel = new Label(string.Empty);

        private readonly ReadOnlyCollection<IDmsPropertyEntry> _propertyEntries;

        public PropertyDialog(IEngine engine, ReadOnlyCollection<IDmsPropertyEntry> propertyEntries, string propertyName, string savedCommentsValue = null, bool showCancelButton = true) : base(engine)
        {
            Title = "Set Property";

            _propertyEntries = propertyEntries;

            if (!showCancelButton)
            {
                CancelButton.IsVisible = false;
            }

            Initialize(propertyName, savedCommentsValue);
            GenerateUI();
        }

        public TextBox MessageTextBox { get; private set; } = new TextBox(String.Empty) { IsMultiline = true, MinHeight = 100 };

        public DropDown MessageDropdown { get; private set; } = new DropDown();

        public Button OkButton { get; private set; } = new Button("OK");

        public Button CancelButton { get; private set; } = new Button("Cancel");

        public void SetComment(string newCommentValue)
        {
            MessageTextBox.Text = newCommentValue;
        }

        private void Initialize(string propertyName, string savedCommentsValue)
        {
            propertyValueLabel.Text = propertyName;

            if (_propertyEntries != null && _propertyEntries.Any())
            {
                var propertyEntries = _propertyEntries.Select(x => x.Value).ToList();
                if (string.IsNullOrWhiteSpace(savedCommentsValue))
                {
                    propertyEntries.Insert(0, string.Empty);
                }
                else if (!string.IsNullOrWhiteSpace(savedCommentsValue) && !MessageDropdown.Options.Contains(savedCommentsValue))
                {
                    propertyEntries.Add(savedCommentsValue);

                    warningLabel.Text = $"Currently a value which is not supported by default is saved on this property.";
                }

                MessageDropdown.Options = propertyEntries;
                MessageDropdown.Selected = savedCommentsValue;
            }
            else
            {
                MessageTextBox.Text = savedCommentsValue;
            }
        }

        private void GenerateUI()
        {
            int row = -1;

            if (!string.IsNullOrWhiteSpace(warningLabel.Text))
            {
                AddWidget(warningLabel, ++row, 0, 1, 2);
            }

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(propertyValueLabel, ++row, 0);
            if (_propertyEntries != null && _propertyEntries.Any())
            {
                AddWidget(MessageDropdown, row, 1);
            }
            else
            {
                AddWidget(MessageTextBox, row, 1);
            }

            row += row + 4;

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(OkButton, ++row, 0);
            AddWidget(CancelButton, row, 1);
        }
    }
}
