
using System.Timers;
using System.Windows;
using System.Windows.Media;
/*
 *Basic info of Plugins
 *Assembly->Package->Plugs
 *One Package for single assembly, including several plug-ins.
 */
namespace MyToolBar.Plugin;
public enum PluginType
{
    /// <summary>
    /// λ��ToolBar�м������OuterControl
    /// </summary>
    OuterControl,
    /// <summary>
    /// λ��ToolBar�Ҳ��Capsule
    /// </summary>
    Capsule,
    /// <summary>
    /// ȫ���Զ������
    /// </summary>
    UserService
}
public interface IPackage
{
    /// <summary>
    /// ������
    /// </summary>
    string PackageName { get; }
    /// <summary>
    /// ����
    /// </summary>
    string Description { get; }
    /// <summary>
    /// ���汾
    /// </summary>
    Version Version { get; }
    /// <summary>
    /// �����Ĳ��
    /// </summary>
    List<IPlugin> Plugins { get; }
}
public interface IPlugin
{
    /// <summary>
    /// �����İ� ������������������
    /// </summary>
    IPackage? AcPackage { get; set; }
    /// <summary>
    /// �������
    /// </summary>
    string Name { get; }
    /// <summary>
    /// �������
    /// </summary>
    string Description { get; }
    /// <summary>
    /// �йܵ�����SignKeys
    /// </summary>
    List<string>? SettingsSignKeys { get; }
    /// <summary>
    /// �������
    /// </summary>
    PluginType Type { get; }
    /// <summary>
    /// ���������ṩ����ҪUIElement
    /// </summary>
    /// <returns></returns>
    virtual UIElement? GetMainElement() => null;
    virtual ServiceBase? GetServiceHost() => null;
}
