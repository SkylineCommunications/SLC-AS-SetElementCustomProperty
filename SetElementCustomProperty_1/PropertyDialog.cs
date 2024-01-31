using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetElementCustomProperty_1
{
    using System;

    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class PropertyDialog : Dialog
    {
        private readonly Label propertyValueLabel = new Label();

        public PropertyDialog(IEngine engine, string propertyName, string savedCommentsValue = null, bool showCancelButton = true) : base(engine)
        {
            Title = "Set Property";

            if (!showCancelButton)
            {
                CancelButton.IsVisible = false;
            }

            Initialize(propertyName, savedCommentsValue);
            GenerateUI();
        }

        public TextBox MessageTextBox { get; private set; } = new TextBox(String.Empty) { IsMultiline = true, MinHeight = 100 };

        public Button OkButton { get; private set; } = new Button("OK");

        public Button CancelButton { get; private set; } = new Button("Cancel");

        public void SetComment(string newCommentValue)
        {
            MessageTextBox.Text = newCommentValue;
        }

        private void Initialize(string propertyName, string savedCommentsValue)
        {
            propertyValueLabel.Text = propertyName;
            MessageTextBox.Text = savedCommentsValue;
        }

        private void GenerateUI()
        {
            int row = -1;

            AddWidget(propertyValueLabel, ++row, 0);
            AddWidget(MessageTextBox, row, 1);

            row += row + 4;

            AddWidget(new WhiteSpace(), ++row, 0);

            AddWidget(OkButton, ++row, 0);
            AddWidget(CancelButton, row, 1);
        }
    }
}
