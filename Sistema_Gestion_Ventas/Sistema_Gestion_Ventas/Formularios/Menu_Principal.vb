Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Templates
Imports DevExpress.ExpressApp.Templates.ActionControls
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Win.Controls
Imports DevExpress.ExpressApp.Win.SystemModule
Imports DevExpress.ExpressApp.Win.Templates
Imports DevExpress.ExpressApp.Win.Templates.Utils
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Docking
Imports DevExpress.XtraBars.Docking2010
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.XtraEditors

Partial Public Class Menu_Principal
    Inherits RibbonForm
    Implements IActionControlsSite
    Implements IContextMenuHolder
    Implements IWindowTemplate
    Implements IDockManagerHolder
    Implements IBarManagerHolder
    Implements ISupportViewChanged
    Implements IXafDocumentsHostWindow
    Implements ISupportUpdate
    Implements IViewSiteTemplate
    Implements ISupportStoreSettings
    Implements IViewHolder
    Implements INavigationPanelHolder
    Private Shared ReadOnly viewChanged As New Object()
    Private Shared ReadOnly settingsReloaded As New Object()
    Private uiType As UIType
    Private statusMessagesHelper As StatusMessagesHelper

    Protected Overridable Sub InitializeImages()
        barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo("Action_WindowList").Image
        barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_WindowList").Image
        barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo("Action_Navigation").Image
        barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_Navigation").Image
    End Sub
    Protected Overridable Sub OnUITypeChanged()
        Dim uiType__1 As UIType = DirectCast(Me, IXafDocumentsHostWindow).UIType
        If uiType__1 = uiType.TabbedMDI Then
            SetupTabbedMdi()
        ElseIf uiType__1 = uiType.StandardMDI Then
            SetupStandardMdi()
        Else
            SetupSdi()
        End If
    End Sub
    Protected Sub SetupSdi()
        ribbonControl.MdiMergeStyle = RibbonMdiMergeStyle.Never
        documentManager.View = noDocumentsView
        documentManager.ViewCollection.Remove(nativeMdiView)
        documentManager.ViewCollection.Remove(tabbedView)
        viewSitePanel.Visible = True
        documentManager.ClientControl = viewSitePanel
        barMdiChildrenListItem.Visibility = BarItemVisibility.Never
    End Sub
    Protected Sub SetupStandardMdi()
        ribbonControl.MdiMergeStyle = RibbonMdiMergeStyle.OnlyWhenMaximized
        documentManager.View = nativeMdiView
        documentManager.ViewCollection.Remove(noDocumentsView)
        documentManager.ViewCollection.Remove(tabbedView)
        SetupMdiCommon()
    End Sub
    Protected Sub SetupTabbedMdi()
        ribbonControl.MdiMergeStyle = RibbonMdiMergeStyle.Always
        documentManager.View = tabbedView
        documentManager.ViewCollection.Remove(noDocumentsView)
        documentManager.ViewCollection.Remove(nativeMdiView)
        SetupMdiCommon()
    End Sub
    Private Sub SetupMdiCommon()
        viewSitePanel.Visible = False
        documentManager.MdiParent = Me
        barMdiChildrenListItem.Visibility = BarItemVisibility.Always
    End Sub

    Protected Overridable Sub RaiseViewChanged(view As DevExpress.ExpressApp.View)
        RaiseEvent ISupportViewChanged_ViewChanged(Me, New TemplateViewChangedEventArgs(view))
    End Sub
    Protected Overridable Sub RaiseSettingsReloaded()
        RaiseEvent ISupportStoreSettings_SettingsReloaded(Me, EventArgs.Empty)
    End Sub

    Protected Overrides ReadOnly Property ShowMode() As FormShowMode
        Get
            Return FormShowMode.AfterInitialization
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
        InitializeImages()
        ribbonControl.Manager.ForceLinkCreate()
        statusMessagesHelper = New StatusMessagesHelper(barContainerStatusMessages)
        OnUITypeChanged()
    End Sub

#Region "IActionControlsSite Members"
    Private ReadOnly Property IActionControlsSite_ActionContainers() As IEnumerable(Of IActionControlContainer) Implements IActionControlsSite.ActionContainers
        Get
            Return ribbonControl.ActionContainers
        End Get
    End Property
    Private ReadOnly Property IActionControlsSite_ActionControls() As IEnumerable(Of IActionControl) Implements IActionControlsSite.ActionControls
        Get
            Return ribbonControl.ActionControls
        End Get
    End Property
    Private ReadOnly Property IActionControlsSite_DefaultContainer() As IActionControlContainer Implements IActionControlsSite.DefaultContainer
        Get
            Return barActionContainerDefault
        End Get
    End Property
#End Region

#Region "IFrameTemplate Members"
    Private Sub IFrameTemplate_SetView(view As DevExpress.ExpressApp.View) Implements IFrameTemplate.SetView
        viewSiteManager.SetView(view)
        RaiseViewChanged(view)
    End Sub
    Private Function IFrameTemplate_GetContainers() As ICollection(Of IActionContainer) Implements IFrameTemplate.GetContainers
        Return New IActionContainer() {navigation}
    End Function
    Private ReadOnly Property IFrameTemplate_DefaultContainer() As IActionContainer Implements IFrameTemplate.DefaultContainer
        Get
            Return Nothing
        End Get
    End Property
#End Region

#Region "IWindowTemplate Members"
    Private Sub IWindowTemplate_SetCaption(caption As String) Implements IWindowTemplate.SetCaption
        ribbonControl.ApplicationCaption = " "
        ribbonControl.ApplicationDocumentCaption = caption
    End Sub
    Private Sub IWindowTemplate_SetStatus(statusMessages As ICollection(Of String)) Implements IWindowTemplate.SetStatus
        statusMessagesHelper.SetMessages(statusMessages)
    End Sub
    Private Property IWindowTemplate_IsSizeable() As Boolean Implements IWindowTemplate.IsSizeable
        Get
            Return FormBorderStyle.Sizable = FormBorderStyle
        End Get
        Set(value As Boolean)
            FormBorderStyle = (If(value, FormBorderStyle.Sizable, FormBorderStyle.FixedDialog))
        End Set
    End Property
#End Region

#Region "IBarManagerHolder Members"
    Private ReadOnly Property IBarManagerHolder_BarManager() As BarManager Implements IBarManagerHolder.BarManager
        Get
            Return ribbonControl.Manager
        End Get
    End Property
    Private Custom Event IBarManagerHolder_BarManagerChanged As EventHandler Implements IBarManagerHolder.BarManagerChanged
        AddHandler(ByVal value As EventHandler)
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
        End RaiseEvent
    End Event
#End Region

#Region "IDockManagerHolder Members"
    Private ReadOnly Property IDockManagerHolder_DockManager() As DevExpress.XtraBars.Docking.DockManager Implements IDockManagerHolder.DockManager
        Get
            Return dockManager
        End Get
    End Property
#End Region

#Region "IContextMenuHolder"
    Private ReadOnly Property IContextMenuHolder_ContextMenu() As PopupMenu Implements IContextMenuHolder.ContextMenu
        Get
            Return templateContextMenu
        End Get
    End Property
#End Region

#Region "ISupportViewChanged Members"
    Private Custom Event ISupportViewChanged_ViewChanged As EventHandler(Of TemplateViewChangedEventArgs) Implements ISupportViewChanged.ViewChanged
        AddHandler(ByVal value As EventHandler(Of TemplateViewChangedEventArgs))
            Events.AddHandler(viewChanged, value)
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of TemplateViewChangedEventArgs))
            Events.RemoveHandler(viewChanged, value)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As TemplateViewChangedEventArgs)
            Dim handler As EventHandler(Of TemplateViewChangedEventArgs) = CType(Events(viewChanged), EventHandler(Of TemplateViewChangedEventArgs))
            If (handler IsNot Nothing) Then
                handler.Invoke(sender, e)
            End If
        End RaiseEvent
    End Event
#End Region

#Region "IDocumentsHostWindow Members"
    Private Sub IDocumentsHostWindow_Show() Implements IDocumentsHostWindow.Show
        Show()
    End Sub
    Private Sub IDocumentsHostWindow_Hide() Implements IDocumentsHostWindow.Hide
        Hide()
    End Sub
    Private Sub IDocumentsHostWindow_Close() Implements IDocumentsHostWindow.Close
        Close()
    End Sub
    Private ReadOnly Property IDocumentsHostWindow_DestroyOnRemovingChildren() As Boolean Implements IDocumentsHostWindow.DestroyOnRemovingChildren
        Get
            Return True
        End Get
    End Property
    Private ReadOnly Property IDocumentsHostWindow_DocumentManager() As DocumentManager Implements IDocumentsHostWindow.DocumentManager
        Get
            Return documentManager
        End Get
    End Property
    Private Custom Event IDocumentsHostWindow_Closing As CancelEventHandler Implements IDocumentsHostWindow.Closing
        AddHandler(ByVal value As CancelEventHandler)
            AddHandler Closing, value
        End AddHandler
        RemoveHandler(ByVal value As CancelEventHandler)
            RemoveHandler Closing, value
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As CancelEventArgs)
        End RaiseEvent
    End Event
    Private Custom Event IDocumentsHostWindow_Closed As EventHandler Implements IDocumentsHostWindow.Closed
        AddHandler(ByVal value As EventHandler)
            AddHandler Closed, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler Closed, value
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
        End RaiseEvent
    End Event
#End Region

#Region "IXafDocumentsHostWindow Members"
    Private Property IXafDocumentsHostWindow_UIType() As UIType Implements IXafDocumentsHostWindow.UIType
        Get
            Return uiType
        End Get
        Set(value As UIType)
            If uiType <> value Then
                uiType = value
                OnUITypeChanged()
            End If
        End Set
    End Property
#End Region

#Region "ISupportUpdate Members"
    Private Sub ISupportUpdate_BeginUpdate() Implements ISupportUpdate.BeginUpdate
        If ribbonControl.Manager IsNot Nothing Then
            ribbonControl.Manager.BeginUpdate()
        End If
    End Sub
    Private Sub ISupportUpdate_EndUpdate() Implements ISupportUpdate.EndUpdate
        If ribbonControl.Manager IsNot Nothing Then
            ribbonControl.Manager.EndUpdate()
        End If
    End Sub
#End Region

#Region "IViewSiteTemplate Members"
    Private ReadOnly Property IViewSiteTemplate_ViewSiteControl() As Object Implements IViewSiteTemplate.ViewSiteControl
        Get
            Return viewSiteManager.ViewSiteControl
        End Get
    End Property
#End Region

#Region "ISupportStoreSettings Members"
    Private Sub ISupportStoreSettings_SetSettings(settings As IModelTemplate) Implements ISupportStoreSettings.SetSettings
        Dim templateModel As IModelTemplateWin = DirectCast(settings, IModelTemplateWin)
        Dim templatesHelper As New TemplatesHelper(templateModel)
        formStateModelSynchronizer.Model = templatesHelper.GetFormStateNode()
        navigation.Model = templatesHelper.GetNavBarCustomizationNode()
        templatesHelper.SetRibbonSettings(ribbonControl)
    End Sub
    Private Sub ISupportStoreSettings_ReloadSettings() Implements ISupportStoreSettings.ReloadSettings
        modelSynchronizationManager.ApplyModel()
        RaiseSettingsReloaded()
    End Sub
    Private Sub ISupportStoreSettings_SaveSettings() Implements ISupportStoreSettings.SaveSettings
        SuspendLayout()
        Try
            modelSynchronizationManager.SynchronizeModel()
        Finally
            ResumeLayout()
        End Try
    End Sub
    Private Custom Event ISupportStoreSettings_SettingsReloaded As EventHandler Implements ISupportStoreSettings.SettingsReloaded
        AddHandler(ByVal value As EventHandler)
            Events.AddHandler(settingsReloaded, value)
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            Events.RemoveHandler(settingsReloaded, value)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            Dim handler As EventHandler = CType(Events(settingsReloaded), EventHandler)
            If (handler IsNot Nothing) Then
                handler.Invoke(sender, e)
            End If
        End RaiseEvent
    End Event
#End Region

#Region "IViewHolder Members"
    Private ReadOnly Property IViewHolder_View() As DevExpress.ExpressApp.View Implements IViewHolder.View
        Get
            Return viewSiteManager.View
        End Get
    End Property
#End Region

#Region "INavigationPanelHolder Members"
    Private ReadOnly Property INavigationPanelHolder_DockPanelNavigation() As DockPanel Implements INavigationPanelHolder.DockPanelNavigation
        Get
            Return dockPanelNavigation
        End Get
    End Property

    Private Sub Menu_Principal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Llenar_Combobox("Select Cast(table_Name as Varchar)'Tablas' From Information_Schema.Tables", "Tablas", Me.BarEditItem1)
    End Sub

    Private Sub BarButtonItem1_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItem1.ItemClick
        If BarEditItem1.EditValue <> "" Then
            Importar_Doc(Me.BarEditItem1.EditValue, Migracion_Base_Datos.DataGridView1)
            Cargar_Formularios(Me, Migracion_Base_Datos)
        Else
            MessageBox.Show("Debe seleccionar la tabla que desea importar", "Información", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub XtraTabbedMdiManager1_PageAdded(sender As Object, e As DevExpress.XtraTabbedMdi.MdiTabPageEventArgs) Handles XtraTabbedMdiManager1.PageAdded
        Gestionar_Formularios(Me.XtraTabbedMdiManager1, Me.viewSitePanel)
    End Sub

    Private Sub XtraTabbedMdiManager1_PageRemoved(sender As Object, e As DevExpress.XtraTabbedMdi.MdiTabPageEventArgs) Handles XtraTabbedMdiManager1.PageRemoved
        Gestionar_Formularios(Me.XtraTabbedMdiManager1, Me.viewSitePanel)
    End Sub
#End Region
End Class
