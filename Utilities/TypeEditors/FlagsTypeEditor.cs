using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Utilities.Extansions.EnumExtansions;

namespace Utilities.TypeEditors
{
	/// <summary>
	/// UI type editor for flags enumerations, that enables the user to select multiple values for the enum.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	public class FlagsTypeEditor<TEnum> : UITypeEditor
		where TEnum : struct
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether drop-down editors should be resizable by the user.
		/// </summary>
		public override bool IsDropDownResizable => true; 
		#endregion

		#region Methods

		/// <summary>
		/// Edits the specified object's value using the editor style indicated by the GetEditStyle method.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
		/// <param name="provider">An IServiceProvider that this editor can use to obtain services.</param>
		/// <param name="value">The object to edit.</param>
		/// <returns>The new value of the object. If the value of the object has not changed, this should return the same object it was passed.</returns>
		public override object EditValue(
			ITypeDescriptorContext context,
			IServiceProvider provider,
			object value)
		{
			Type type = value.GetType();

			if (type.FullName != typeof(TEnum).FullName)
			{
				throw new ArgumentException(string.Format(" {0}\nActual: {1}", typeof(TEnum), type));
			}

			if (!type.IsEnum ||
				!type.CustomAttributes.Any(customAttr =>
					customAttr.AttributeType == typeof(FlagsAttribute)))
			{
				throw new InvalidEnumArgumentException(nameof(value));
			}

			var winFormsSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			TEnum enumValue = (TEnum)(value ?? default(TEnum));

			if (winFormsSvc != null)
			{
				TEnum fullSelection = Combine(GetValues<TEnum>());

				using (var control = new UserControl() { BackColor = SystemColors.Control })
				{
					var listbox = new CheckedListBox()
					{
						Dock = DockStyle.Fill,
						CheckOnClick = true,
					};

					var listValues = GetValues<TEnum>().Select(val =>
						new
						{
							Object = val,
							IsChecked = HasFlag(enumValue, val),
						});

					listbox.Items.Add("Select all",
						fullSelection.Equals(value));

					foreach (var item in listValues)
					{
						listbox.Items.Add(item.Object, item.IsChecked);
					}

					OperationFlag isChecking = false;

					listbox.ItemCheck += (s, e) =>
						{
							if (!isChecking)
							{
								using (isChecking.Flip())
								{
									if (e.Index == 0)
									{
										for (int i = 1; i < listbox.Items.Count; i++)
										{
											listbox.SetItemChecked(i, (e.NewValue == CheckState.Checked) ^
												((TEnum)listbox.Items[i]).Equals(default(TEnum)));
										}

										value = e.NewValue == CheckState.Checked
											? fullSelection
											: default(TEnum);
									}
									else
									{
										var updatedValues = listbox.Items.Cast<object>()
																		 .Select((Item, Index) => new { Item, Index })
																		 .Where(item => (item.Index == e.Index && e.NewValue == CheckState.Checked) ||
																						(listbox.GetItemChecked(item.Index) && (item.Index != e.Index)))
																		 .Select(item => item.Item)
																		 .OfType<TEnum>();

										value = Combine(updatedValues);
										listbox.SetItemChecked(0, fullSelection.Equals(value));
									}
								}

							}
						};

					var okButton = new Button
					{
						Text = "Ok",
						Dock = DockStyle.Bottom,
						BackColor = SystemColors.ButtonFace,
					};

					okButton.Click += delegate
					{
						winFormsSvc.CloseDropDown();
					};

					control.Controls.Add(listbox);
					control.Controls.Add(okButton);

					winFormsSvc.DropDownControl(control);
				}
			}

			return value;
		}

		/// <summary>
		/// Gets the editor style used by the EditValue method.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
		/// <returns>
		/// A UITypeEditorEditStyle value that indicates the style of editor used by the EditValue method. 
		/// If the UITypeEditor does not support this method, then GetEditStyle will return None.
		/// </returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) =>
			UITypeEditorEditStyle.DropDown;
		#endregion
	}
}
