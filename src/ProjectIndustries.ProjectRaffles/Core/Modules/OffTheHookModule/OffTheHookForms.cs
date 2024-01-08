using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.OffTheHookModule
{
    public class OffTheHookForms
    {
        public class Form
    {
        [JsonProperty("live_form_versions")]
        public List<LiveFormVersion> LiveFormVersions { get; set; }

        [JsonProperty("form_id")]
        public string FormId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class LiveFormVersion
    {
        [JsonProperty("allocation")]
        public long Allocation { get; set; }

        [JsonProperty("trigger_groups")]
        public List<TriggerGroup> TriggerGroups { get; set; }

        [JsonProperty("form_type_direction")]
        public object FormTypeDirection { get; set; }

        [JsonProperty("form_id")]
        public string FormId { get; set; }

        [JsonProperty("update_timestamp")]
        public long UpdateTimestamp { get; set; }

        [JsonProperty("views")]
        public List<View> Views { get; set; }

        [JsonProperty("form_type")]
        public string FormType { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("form_version_id")]
        public long FormVersionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class TriggerGroup
    {
        [JsonProperty("trigger_group_id")]
        public long TriggerGroupId { get; set; }

        [JsonProperty("form_version_id")]
        public long FormVersionId { get; set; }

        [JsonProperty("triggers")]
        public List<Trigger> Triggers { get; set; }

        [JsonProperty("SCROLL_PERCENTAGE", NullValueHandling = NullValueHandling.Ignore)]
        public ScrollPercentage ScrollPercentage { get; set; }

        [JsonProperty("SUPPRESS_SUCCESS_FORM", NullValueHandling = NullValueHandling.Ignore)]
        public ExistingUser SuppressSuccessForm { get; set; }

        [JsonProperty("EXISTING_USER", NullValueHandling = NullValueHandling.Ignore)]
        public ExistingUser ExistingUser { get; set; }

        [JsonProperty("EXIT_INTENT", NullValueHandling = NullValueHandling.Ignore)]
        public ExistingUser ExitIntent { get; set; }
    }

    public partial class ExistingUser
    {
        [JsonProperty("value")]
        public bool Value { get; set; }
    }

    public partial class ScrollPercentage
    {
        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class Trigger
    {
        [JsonProperty("trigger_group_id")]
        public long TriggerGroupId { get; set; }

        [JsonProperty("trigger_id")]
        public long TriggerId { get; set; }

        [JsonProperty("trigger_type")]
        public string TriggerType { get; set; }
    }
    

    public partial class View
    {
        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("view_id")]
        public long ViewId { get; set; }

        [JsonProperty("data")]
        public ViewData Data { get; set; }

        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }

        [JsonProperty("form_version_id")]
        public long FormVersionId { get; set; }
    }

    public partial class Column
    {
        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("view_id")]
        public long ViewId { get; set; }

        [JsonProperty("rows")]
        public List<Row> Rows { get; set; }

        [JsonProperty("data")]
        public ColumnData Data { get; set; }

        [JsonProperty("column_id")]
        public long ColumnId { get; set; }
    }

    public partial class ColumnData
    {
    }

    public partial class Row
    {
        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("row_id")]
        public long RowId { get; set; }

        [JsonProperty("data")]
        public ColumnData Data { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }

        [JsonProperty("column_id")]
        public long ColumnId { get; set; }
    }

    public partial class Component
    {
        [JsonProperty("action")]
        public Action Action { get; set; }

        [JsonProperty("row_id")]
        public long RowId { get; set; }

        [JsonProperty("component_id")]
        public long ComponentId { get; set; }

        [JsonProperty("data")]
        public ComponentData Data { get; set; }

    }

    public partial class Action
    {
        [JsonProperty("view_id")]
        public long ViewId { get; set; }

        [JsonProperty("list_id")]
        public string ListId { get; set; }

        [JsonProperty("data")]
        public ColumnData Data { get; set; }

        [JsonProperty("action_type")]
        public string ActionType { get; set; }

        [JsonProperty("action_id")]
        public long ActionId { get; set; }
    }

    public partial class ComponentData
    {
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public Image Image { get; set; }

        [JsonProperty("styling")]
        public PurpleStyling Styling { get; set; }

        [JsonProperty("placeholder", NullValueHandling = NullValueHandling.Ignore)]
        public string Placeholder { get; set; }

        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DataRequired { get; set; }

        [JsonProperty("field_id", NullValueHandling = NullValueHandling.Ignore)]
        public string FieldId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("prefill", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Prefill { get; set; }

        [JsonProperty("selected_country_code", NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedCountryCode { get; set; }

        [JsonProperty("is_updating_s_m_s_consent", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsUpdatingSMSConsent { get; set; }
        

        [JsonProperty("delimiter", NullValueHandling = NullValueHandling.Ignore)]
        public string Delimiter { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<Option> Options { get; set; }

        [JsonProperty("meta_fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<MetaField> MetaFields { get; set; }

        [JsonProperty("alt_text", NullValueHandling = NullValueHandling.Ignore)]
        public string AltText { get; set; }
    }

    public partial class ContentClass
    {
        [JsonProperty("ops")]
        public List<Op> Ops { get; set; }
    }

    public partial class Op
    {
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public Attributes Attributes { get; set; }

        [JsonProperty("insert")]
        public string Insert { get; set; }
    }

    public partial class Attributes
    {
        [JsonProperty("bold", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Bold { get; set; }

        [JsonProperty("italic", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Italic { get; set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public Uri Link { get; set; }

        [JsonProperty("underline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Underline { get; set; }

        [JsonProperty("background", NullValueHandling = NullValueHandling.Ignore)]
        public string Background { get; set; }
    }

    public partial class Image
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }

    public partial class MetaField
    {
        [JsonProperty("field_id")]
        public string FieldId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Option
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }

    public partial class PurpleStyling
    {
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public long? Width { get; set; }

        [JsonProperty("full_width", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FullWidth { get; set; }

        [JsonProperty("text_styles", NullValueHandling = NullValueHandling.Ignore)]
        public TextStyles TextStyles { get; set; }
        

        [JsonProperty("padding", NullValueHandling = NullValueHandling.Ignore)]
        public Padding Padding { get; set; }

        [JsonProperty("background_color", NullValueHandling = NullValueHandling.Ignore)]
        public string BackgroundColor { get; set; }

        [JsonProperty("specify_hover_background_color", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpecifyHoverBackgroundColor { get; set; }

        [JsonProperty("hover_text_color", NullValueHandling = NullValueHandling.Ignore)]
        public string HoverTextColor { get; set; }

        [JsonProperty("border_width", NullValueHandling = NullValueHandling.Ignore)]
        public long? BorderWidth { get; set; }

        [JsonProperty("inner_alignment", NullValueHandling = NullValueHandling.Ignore)]
        public string InnerAlignment { get; set; }

        [JsonProperty("arrangement", NullValueHandling = NullValueHandling.Ignore)]
        public string Arrangement { get; set; }

        [JsonProperty("border_style", NullValueHandling = NullValueHandling.Ignore)]
        public string BorderStyle { get; set; }

        [JsonProperty("border_radius", NullValueHandling = NullValueHandling.Ignore)]
        public long? BorderRadius { get; set; }

        [JsonProperty("hover_background_color", NullValueHandling = NullValueHandling.Ignore)]
        public string HoverBackgroundColor { get; set; }
    }

    public partial class Padding
    {
        [JsonProperty("top", NullValueHandling = NullValueHandling.Ignore)]
        public long? Top { get; set; }

        [JsonProperty("bottom", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bottom { get; set; }

        [JsonProperty("right", NullValueHandling = NullValueHandling.Ignore)]
        public long? Right { get; set; }

        [JsonProperty("left", NullValueHandling = NullValueHandling.Ignore)]
        public long? Left { get; set; }
    }

    public partial class TextStyles
    {
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty("font_family", NullValueHandling = NullValueHandling.Ignore)]
        public string FontFamily { get; set; }
    }

    public partial class ViewData
    {
        [JsonProperty("styling")]
        public FluffyStyling Styling { get; set; }
    }

    public partial class FluffyStyling
    {
        [JsonProperty("padding", NullValueHandling = NullValueHandling.Ignore)]
        public Padding Padding { get; set; }

        [JsonProperty("preset_size")]
        public bool PresetSize { get; set; }

        [JsonProperty("is_max_width")]
        public bool IsMaxWidth { get; set; }

        [JsonProperty("input_styles")]
        public InputStyles InputStyles { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("background_image")]
        public object BackgroundImage { get; set; }

        [JsonProperty("border_radius", NullValueHandling = NullValueHandling.Ignore)]
        public long? BorderRadius { get; set; }
    }

    public partial class InputStyles
    {

        [JsonProperty("focus_color", NullValueHandling = NullValueHandling.Ignore)]
        public string FocusColor { get; set; }

        [JsonProperty("font_family", NullValueHandling = NullValueHandling.Ignore)]
        public string FontFamily { get; set; }
    }
    }
}