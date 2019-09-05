using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AbpCompanyName.AbpProjectName.Ui.Dto
{
    public class ElementUiTableColumnDto
    {
        public ElementUiTableColumnDto()
        {
        }

        public ElementUiTableColumnDto(string lable, string prop, string type = "text", bool showOverflowTooltip = true, bool sortable = true, string width = "", CssAlign? align = null)
        {
            Label = lable;
            Prop = prop;
            Type = type;
            ShowOverflowTooltip = showOverflowTooltip;
            Sortable = sortable;
            Width = width;
            if (align.HasValue)
            {
                Align = align.Value;
            }
        }

        /// <summary>
        /// 显示的标题
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 对应列内容的字段名，也可以使用 property 属性
        /// </summary>
        public string Prop { get; set; }

        public string Type { get; set; }
        public bool ShowOverflowTooltip { get; set; }
        public bool Sortable { get; set; }

        public string Width { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CssAlign Align { get; set; }
    }
}