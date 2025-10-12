using System;
using System.Collections.Generic;

namespace CreateGDAPI
{
    /// <summary>
    /// Configuration for field selection and modes
    /// </summary>
    public class FieldsConfig
    {
        public List<string> SelectedFields { get; set; } = new();
        public bool UseBlackListOnly { get; set; } = false;
        public Dictionary<string, FieldMode> FieldModes { get; set; } = new();
    }

    /// <summary>
    /// Field mode for controlling how fields are sent in requests
    /// </summary>
    public enum FieldMode
    {
        Normal = 0,      // Send random data (default)
        SendNull = 1,    // Send null value
        NotSend = 2      // Do not send this field
    }
}