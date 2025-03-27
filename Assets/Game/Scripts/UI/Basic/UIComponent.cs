using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

public class UIComponent
{

    public string nodeName = "";

    private GameObject obj = null;
    public UIComponent(GameObject obj, string name)
    {
        this.obj = obj;
        this.nodeName = name;
    }

    public UIComponent(GameObject obj)
    {
        this.obj = obj;
    }

    public GameObject gameObject
    {
        get
        {
            return obj;
        }
    }

    public void SetActive(bool active)
    {
        obj.SetActive(active);
    }

    private Text m_text;
    public Text text
    {
        get
        {
            if (m_text == null)
            {
                if (this.obj == null) return null;
                m_text = this.obj.GetComponent<Text>();
                if (m_text != null)
                {
                    //if (Config.ChannelManager.Instance.SDK_CHANNEL_TYPE == (int)SDK_CHANNEL.韩国_Korea_Google || Config.ChannelManager.Instance.SDK_CHANNEL_TYPE == (int)SDK_CHANNEL.韩国_Korea_IOS ||
                    //Config.ChannelManager.Instance.SDK_CHANNEL_TYPE == (int)SDK_CHANNEL.韩国_Korea_OneStore)
                    //{
                    //     if(m_text.lineSpacing < 1) 
                    //         m_text.lineSpacing = 1;
                    //}
                }
            }

            if (m_text == null)
            {
                //Debug.LogError(this.obj.name + "没有这个组件-》Text");
            }
            return m_text;
        }
    }

    private Button m_button;
    public Button button
    {
        get
        {
            if (m_button == null)
            {
                m_button = obj.GetComponent<Button>();
            }

            if (m_button == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》Button");
            }
            return m_button;
        }
    }

    private Transform m_transform;
    public Transform transform
    {
        get
        {
            if (m_transform == null)
            {
                m_transform = obj.transform;
            }
            return m_transform;
        }
    }

    private string m_name;
    public string name
    {
        get
        {
            if (m_name == null)
            {
                m_name = obj.name;
            }
            return m_name;
        }
    }

    private Camera m_camera;
    public Camera camera
    {
        get
        {
            if (m_camera == null)
            {
                m_camera = obj.GetComponent<Camera>();
            }

            if (m_camera == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》Camera");
            }
            return m_camera;
        }
    }

    private Animator m_animator;
    public Animator animator
    {
        get
        {
            if (m_animator == null)
            {
                m_animator = obj.GetComponent<Animator>();
            }

            if (m_animator == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》Animator");
            }
            return m_animator;
        }
    }

    private Animation m_animation;
    public Animation animation
    {
        get
        {
            if (m_animation == null)
            {
                m_animation = obj.GetComponent<Animation>();
            }

            if (m_animation == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》Animation");
            }
            return m_animation;
        }
    }

    private Collider m_collider;
    public Collider collider
    {
        get
        {
            if (m_collider == null)
            {
                m_collider = obj.GetComponent<Collider>();
            }

            if (m_collider == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》Collider");
            }
            return m_collider;
        }
    }

    private AudioSource m_audio;
    public AudioSource audio
    {
        get
        {
            if (m_audio == null)
            {
                m_audio = obj.GetComponent<AudioSource>();
            }

            if (m_audio == null)
            {
                Debug.LogError(obj.name + "没有这个组件-》AudioSource");
            }
            return m_audio;
        }
    }

    private RectTransform m_rect;
    public RectTransform rect
    {
        get
        {
            if (m_rect == null)
            {
                m_rect = obj.GetComponent<RectTransform>();
            }

            if (m_rect == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_rect;
        }
    }


    private Canvas m_canvas;
    public Canvas canvas
    {
        get
        {
            if (m_canvas == null)
            {
                m_canvas = obj.GetComponent<Canvas>();
            }

            if (m_canvas == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_canvas;
        }
    }

    private CanvasGroup m_canvas_group;
    public CanvasGroup canvas_group
    {
        get
        {
            if (m_canvas_group == null)
            {
                m_canvas_group = obj.GetComponent<CanvasGroup>();
            }

            if (m_canvas_group == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_canvas_group;
        }
    }

    private Image m_image;
    public Image image
    {
        get
        {
            if (m_image == null)
            {
                m_image = obj.GetComponent<Image>();
            }

            //if (m_image == null)
            //{
            //    //Debug.LogError(obj.name + "没有这个组件");
            //}
            return m_image;
        }
    }

    private RawImage m_raw_image;
    public RawImage raw_image
    {
        get
        {
            if (m_raw_image == null)
            {
                m_raw_image = obj.GetComponent<RawImage>();
            }

            if (m_raw_image == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_raw_image;
        }
    }


    private RenderTexture m_render_texture;
    public RenderTexture render_texture
    {
        get
        {
            if (m_render_texture == null)
            {
                m_render_texture = obj.GetComponent<RenderTexture>();
            }

            if (m_render_texture == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_render_texture;
        }
    }

    //private EmojiTxt m_emoji_txt;
    //public EmojiTxt emoji_txt
    //{
    //    get
    //    {
    //        if (m_emoji_txt == null)
    //        {
    //            m_emoji_txt = obj.GetComponent<EmojiTxt>();
    //        }

    //        if (m_emoji_txt == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_emoji_txt;
    //    }
    //}

    private Toggle m_toggle;
    public Toggle toggle
    {
        get
        {
            if (m_toggle == null)
            {
                m_toggle = obj.GetComponent<Toggle>();
            }

            if (m_toggle == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_toggle;
        }
    }

    private ToggleGroup m_toggle_group;
    public ToggleGroup toggle_group
    {
        get
        {
            if (m_toggle_group == null)
            {
                m_toggle_group = obj.GetComponent<ToggleGroup>();
            }

            if (m_toggle == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_toggle_group;
        }
    }

    private Slider m_slider;
    public Slider slider
    {
        get
        {
            if (m_slider == null)
            {
                m_slider = obj.GetComponent<Slider>();
            }

            if (m_slider == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_slider;
        }
    }


    private ScrollRect m_scroll_rect;
    public ScrollRect scroll_rect
    {
        get
        {
            if (m_scroll_rect == null)
            {
                m_scroll_rect = obj.GetComponent<ScrollRect>();
            }

            //if (m_scroll_rect == null)
            //{
            //    Debug.LogError(obj.name + "没有这个组件");
            //}
            return m_scroll_rect;
        }
    }

    private LayoutElement m_layout_element;
    public LayoutElement layout_element
    {
        get
        {
            if (m_layout_element == null)
            {
                m_layout_element = obj.GetComponent<LayoutElement>();
            }

            if (m_layout_element == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_layout_element;
        }
    }

    private InputField m_input_field;
    public InputField input_field
    {
        get
        {
            if (m_input_field == null)
            {
                m_input_field = obj.GetComponent<InputField>();
            }

            if (m_input_field == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_input_field;
        }
    }

    private Dropdown m_dropdown;
    public Dropdown dropdown
    {
        get
        {
            if (m_dropdown == null)
            {
                m_dropdown = obj.GetComponent<Dropdown>();
            }

            if (m_dropdown == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_dropdown;
        }
    }

    //private EnhancedScroller m_scroller;
    //public EnhancedScroller scroller
    //{
    //    get
    //    {
    //        if (m_scroller == null)
    //        {
    //            m_scroller = obj.GetComponent<EnhancedScroller>();
    //        }

    //        if (m_scroller == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_scroller;
    //    }
    //}


    private Shadow m_shadow;
    public Shadow shadow
    {
        get
        {
            if (m_shadow == null)
            {
                m_shadow = obj.GetComponent<Shadow>();
            }

            if (m_shadow == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_shadow;
        }
    }


    private ContentSizeFitter m_size_fitter;
    public ContentSizeFitter size_fitter
    {
        get
        {
            if (m_size_fitter == null)
            {
                m_size_fitter = obj.GetComponent<ContentSizeFitter>();
            }

            if (m_size_fitter == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_size_fitter;
        }
    }


    private Outline m_outline;
    public Outline outline
    {
        get
        {
            if (m_outline == null)
            {
                m_outline = obj.GetComponent<Outline>();
            }

            if (m_outline == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_outline;
        }
    }

    private TextMesh m_textMesh;
    public TextMesh textMesh
    {
        get
        {
            if (m_textMesh == null)
            {
                m_textMesh = obj.GetComponent<TextMesh>();
            }

            if (m_textMesh == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_textMesh;
        }
    }

    //private IconTips m_icontips;
    //public IconTips icontips
    //{
    //    get
    //    {
    //        if (m_icontips == null)
    //        {
    //            m_icontips = obj.GetComponent<IconTips>();
    //        }

    //        if (m_icontips == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_icontips;
    //    }
    //}


    //private QuickGridCenter m_gridcenter;
    //public QuickGridCenter gridcenter
    //{
    //    get
    //    {
    //        if (m_gridcenter == null)
    //        {
    //            m_gridcenter = obj.GetComponent<QuickGridCenter>();
    //        }

    //        if (m_gridcenter == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_gridcenter;
    //    }
    //}


    //private QuickMapGrid m_mapgrid;
    //public QuickMapGrid mapgrid
    //{
    //    get
    //    {
    //        if (m_mapgrid == null)
    //        {
    //            m_mapgrid = obj.GetComponent<QuickMapGrid>();
    //        }

    //        if (m_mapgrid == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_mapgrid;
    //    }
    //}


    //private QuickMapGridItemBase m_mapgriditem;
    //public QuickMapGridItemBase mapgriditem
    //{
    //    get
    //    {
    //        if (m_mapgriditem == null)
    //        {
    //            m_mapgriditem = obj.GetComponent<QuickMapGridItemBase>();
    //        }

    //        if (m_mapgriditem == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_mapgriditem;
    //    }
    //}

    //private DragController m_dragctl;
    //public DragController dragctl
    //{
    //    get
    //    {
    //        if (m_dragctl == null)
    //        {
    //            m_dragctl = obj.GetComponent<DragController>();
    //        }

    //        if (m_dragctl == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_dragctl;
    //    }
    //}

    private EventTrigger m_event_trigger;
    public EventTrigger event_trigger
    {
        get
        {
            if (m_event_trigger == null)
            {
                m_event_trigger = obj.GetComponent<EventTrigger>();
            }

            if (m_event_trigger == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_event_trigger;
        }
    }

    private VerticalLayoutGroup m_ver_layout;
    public VerticalLayoutGroup ver_layout
    {
        get
        {
            if (m_ver_layout == null)
            {
                m_ver_layout = obj.GetComponent<VerticalLayoutGroup>();
            }

            if (m_ver_layout == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_ver_layout;
        }
    }


    private HorizontalLayoutGroup m_hor_layout;
    public HorizontalLayoutGroup hor_layout
    {
        get
        {
            if (m_hor_layout == null)
            {
                m_hor_layout = obj.GetComponent<HorizontalLayoutGroup>();
            }

            if (m_hor_layout == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_hor_layout;
        }
    }

    //private UIPrefabLoader m_prefabloader;
    //public UIPrefabLoader prefabloader
    //{
    //    get
    //    {
    //        if (m_prefabloader == null)
    //        {
    //            m_prefabloader = obj.GetComponent<UIPrefabLoader>();
    //        }

    //        if (m_prefabloader == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_prefabloader;
    //    }
    //}



    //private CurveMove m_curvemove;
    //public CurveMove curvemove
    //{
    //    get
    //    {
    //        if (m_curvemove == null)
    //        {
    //            m_curvemove = obj.GetComponent<CurveMove>();
    //        }

    //        if (m_curvemove == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_curvemove;
    //    }
    //}

    //private ParticleCollect m_particle_col;
    //public ParticleCollect curvemparticle_colove
    //{
    //    get
    //    {
    //        if (m_particle_col == null)
    //        {
    //            m_particle_col = obj.GetComponent<ParticleCollect>();
    //        }

    //        if (m_particle_col == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_particle_col;
    //    }
    //}

    private Gradient m_gradient;
    public Gradient gradient
    {
        get
        {
            if (m_gradient == null)
            {
                m_gradient = obj.GetComponent<Gradient>();
            }

            if (m_gradient == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_gradient;
        }
    }

    //private ListViewCell m_list_cell;
    //public ListViewCell list_cell
    //{
    //    get
    //    {
    //        if (m_list_cell == null)
    //        {
    //            m_list_cell = obj.GetComponent<ListViewCell>();
    //        }

    //        if (m_list_cell == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_list_cell;
    //    }
    //}

    //private ListViewSimpleDelegate m_list_simple_delegate;
    //public ListViewSimpleDelegate list_simple_delegate
    //{
    //    get
    //    {
    //        if (m_list_simple_delegate == null)
    //        {
    //            m_list_simple_delegate = obj.GetComponent<ListViewSimpleDelegate>();
    //        }

    //        if (m_list_simple_delegate == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_list_simple_delegate;
    //    }
    //}

    //private UISetPos m_ui_set_pos;
    //public UISetPos ui_set_pos
    //{
    //    get
    //    {
    //        if (m_ui_set_pos == null)
    //        {
    //            m_ui_set_pos = obj.GetComponent<UISetPos>();
    //        }

    //        if (m_ui_set_pos == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_ui_set_pos;
    //    }
    //}

    //private EffectMove m_effect_move;
    //public EffectMove effect_move
    //{
    //    get
    //    {
    //        if (m_effect_move == null)
    //        {
    //            m_effect_move = obj.GetComponent<EffectMove>();
    //        }

    //        if (m_effect_move == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_effect_move;
    //    }
    //}

    //private SkeletonGraphic m_skeleton_graphic;
    //public SkeletonGraphic skeleton_graphic
    //{
    //    get
    //    {
    //        if (m_skeleton_graphic == null)
    //        {
    //            m_skeleton_graphic = obj.GetComponent<SkeletonGraphic>();
    //        }

    //        if (m_skeleton_graphic == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_skeleton_graphic;
    //    }
    //}


    private UITable m_node_list;
    public UITable node_list
    {
        get
        {
            if (m_node_list == null)
            {
                m_node_list = obj.GetComponent<UITable>();
            }

            if (m_node_list == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_node_list;
        }
    }


    //private ListViewChange m_list_view_change;
    //public ListViewChange list_view_change
    //{
    //    get
    //    {
    //        if (m_list_view_change == null)
    //        {
    //            m_list_view_change = obj.GetComponent<ListViewChange>();
    //        }

    //        if (m_list_view_change == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_list_view_change;
    //    }
    //}


    //private UIChangeColor m_change_color;
    //public UIChangeColor change_color
    //{
    //    get
    //    {
    //        if (m_change_color == null)
    //        {
    //            m_change_color = obj.GetComponent<UIChangeColor>();
    //        }

    //        if (m_change_color == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_change_color;
    //    }
    //}

    //private UIAnimControl m_ui_anim_control;
    //public UIAnimControl ui_anim_control
    //{
    //    get
    //    {
    //        if (m_ui_anim_control == null)
    //        {
    //            m_ui_anim_control = obj.GetComponent<UIAnimControl>();
    //        }

    //        if (m_ui_anim_control == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_ui_anim_control;
    //    }
    //}


    //private QuickGrid m_grid;
    //public QuickGrid grid
    //{
    //    get
    //    {
    //        if (m_grid == null)
    //        {
    //            m_grid = obj.GetComponent<QuickGrid>();
    //        }

    //        if (m_grid == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_grid;
    //    }
    //}


    //private MyToggle m_mytoggle;
    //public MyToggle mytoggle
    //{
    //    get
    //    {
    //        if (m_mytoggle == null)
    //        {
    //            m_mytoggle = obj.GetComponent<MyToggle>();
    //        }

    //        if (m_mytoggle == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_mytoggle;
    //    }
    //}


    //private MyToggleGroup m_mytoggle_group;
    //public MyToggleGroup mytoggle_group
    //{
    //    get
    //    {
    //        if (m_mytoggle_group == null)
    //        {
    //            m_mytoggle_group = obj.GetComponent<MyToggleGroup>();
    //        }

    //        if (m_mytoggle_group == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_mytoggle_group;
    //    }
    //}

    private ListView m_list_view;
    public ListView list_view
    {
        get
        {
            if (m_list_view == null)
            {
                m_list_view = obj.GetComponent<ListView>();
            }

            if (m_list_view == null)
            {
                Debug.LogError(obj.name + "没有这个组件");
            }
            return m_list_view;
        }
    }


    //private RepeatClick m_repeat_clk;
    //public RepeatClick repeat_clk
    //{
    //    get
    //    {
    //        if (m_repeat_clk == null)
    //        {
    //            m_repeat_clk = obj.GetComponent<RepeatClick>();
    //        }

    //        if (m_repeat_clk == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_repeat_clk;
    //    }
    //}


    //private ListViewItem m_list_view_item;
    //public ListViewItem list_view_item
    //{
    //    get
    //    {
    //        if (m_list_view_item == null)
    //        {
    //            m_list_view_item = obj.GetComponent<ListViewItem>();
    //        }

    //        if (m_list_view_item == null)
    //        {
    //            Debug.LogError(obj.name + "没有这个组件");
    //        }
    //        return m_list_view_item;
    //    }
    //}

    private VideoPlayer m_videoPlayer;
    public VideoPlayer videoPlayer
    {
        get
        {
            if (m_videoPlayer == null)
            {
                m_videoPlayer = obj.GetComponent<VideoPlayer>();
            }
            return m_videoPlayer;
        }
    }
}
