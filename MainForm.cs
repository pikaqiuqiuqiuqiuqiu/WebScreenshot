using CefSharp;
using CefSharp.WinForms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace WebScreenshot
{
    public partial class MainForm : Form
    {
        private ChromiumWebBrowser? browser;
        private float dpiScale = 1.0f;

        public MainForm()
        {
            InitializeComponent();
            InitializeBrowser();
            UpdateDpiScale();

            // 初始化时禁用导航按钮
            btnBack.Enabled = false;
            btnForward.Enabled = false;

            this.Load += MainForm_Load;
            this.DpiChanged += MainForm_DpiChanged;
        }

        private void MainForm_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            UpdateDpiScale();
        }

        private void UpdateDpiScale()
        {
            using (Graphics g = this.CreateGraphics())
            {
                dpiScale = g.DpiX / 96.0f;
                Console.WriteLine($"当前DPI缩放比例: {dpiScale}");
            }
        }

        private void InitializeBrowser()
        {
            browser = new ChromiumWebBrowser(txtUrl.Text);
            browser.Dock = DockStyle.Fill;
            
            // 设置自定义的LifeSpanHandler以拦截新窗口的打开
            browser.LifeSpanHandler = new CustomLifeSpanHandler();
            // 设置自定义的上下文菜单处理程序
            browser.MenuHandler = new CustomContextMenuHandler(this);
            
            pnlBrowser.Controls.Add(browser);

            browser.AddressChanged += Browser_AddressChanged;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            
            // 添加加载状态变化的事件处理
            browser.LoadingStateChanged += Browser_UpdateNavigationState;

            // 监听浏览器完成创建事件
            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            
            // 监听导航完成事件
            browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private void Browser_UpdateNavigationState(object? sender, LoadingStateChangedEventArgs e)
        {
            // 更新导航按钮状态
            this.BeginInvoke(new Action(() =>
            {
                UpdateNavigationButtons();
            }));
        }

        private void Browser_IsBrowserInitializedChanged(object? sender, EventArgs e)
        {
            // 浏览器初始化完成后，更新一次按钮状态
            if (browser != null && browser.IsBrowserInitialized)
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateNavigationButtons();
                }));
            }
        }

        private void UpdateNavigationButtons()
        {
            if (browser != null && browser.CanGoBack)
            {
                btnBack.Enabled = true;
            }
            else
            {
                btnBack.Enabled = false;
            }

            if (browser != null && browser.CanGoForward)
            {
                btnForward.Enabled = true;
            }
            else
            {
                btnForward.Enabled = false;
            }
        }

        private void Browser_LoadingStateChanged(object? sender, LoadingStateChangedEventArgs e)
        {
            // 当页面加载完成时，更新地址栏
            if (!e.IsLoading && browser != null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    txtUrl.Text = browser.Address;
                    UpdateNavigationButtons();
                }));
            }
        }

        private void Browser_FrameLoadEnd(object? sender, FrameLoadEndEventArgs e)
        {
            // 仅在主框架导航完成时更新按钮状态
            if (e.Frame.IsMain)
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateNavigationButtons();
                    
                    // 如果地址栏跟当前浏览器地址不一致，更新地址栏
                    if (browser != null && txtUrl.Text != browser.Address)
                    {
                        txtUrl.Text = browser.Address;
                    }
                }));
            }
        }

        // 自定义LifeSpanHandler类，用于控制新窗口的打开方式
        private class CustomLifeSpanHandler : ILifeSpanHandler
        {
            public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                return false;
            }

            public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                // 不需要特殊处理
            }

            public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                // 不需要特殊处理
            }

            public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                // 拦截新窗口的打开，在当前窗口中加载链接
                if (userGesture && !string.IsNullOrEmpty(targetUrl))
                {
                    chromiumWebBrowser.Load(targetUrl);
                }

                newBrowser = null!;
                return true; // 返回true表示取消默认的弹出窗口行为
            }
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            NavigateToUrl(txtUrl.Text);
            UpdateNavigationButtons();
        }

        private void Browser_AddressChanged(object? sender, AddressChangedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                txtUrl.Text = e.Address;
                UpdateNavigationButtons();
            }));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            NavigateToUrl(txtUrl.Text);
        }

        private void txtUrl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                NavigateToUrl(txtUrl.Text);
            }
        }

        // 前进按钮点击事件
        private void btnForward_Click(object sender, EventArgs e)
        {
            if (browser != null && browser.CanGoForward)
            {
                // 显式调用CEF浏览器的前进方法
                browser.GetBrowser().GoForward();
                
                // 更新按钮状态
                UpdateNavigationButtons();
            }
        }

        // 后退按钮点击事件
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (browser != null && browser.CanGoBack)
            {
                // 显式调用CEF浏览器的后退方法
                browser.GetBrowser().GoBack();
                
                // 更新按钮状态
                UpdateNavigationButtons();
            }
        }

        public void NavigateToUrl(string url)
        {
            if (browser == null) return;

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                if (IsValidDomainOrIP(url))
                {
                    url = "https://" + url;
                }
                else
                {
                    url = "https://www.baidu.com/s?wd=" + Uri.EscapeDataString(url);
                }
            }

            browser.Load(url);
        }

        private bool IsValidDomainOrIP(string url)
        {
            string pattern = @"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$|^(\d{1,3}\.){3}\d{1,3}$";
            return Regex.IsMatch(url, pattern);
        }

        private async void btnScreenshot_Click(object sender, EventArgs e)
        {
            if (browser == null) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                
                var bitmap = await CaptureFullPageAsync();
                
                if (bitmap != null)
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "PNG图片|*.png";
                    saveDialog.Title = "保存截图";
                    saveDialog.DefaultExt = "png";
                    saveDialog.FileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bitmap.Save(saveDialog.FileName, ImageFormat.Png);
                        MessageBox.Show($"截图已保存到：{saveDialog.FileName}", "保存成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    bitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"截图失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task<Bitmap?> CaptureFullPageAsync()
        {
            if (browser == null) return null;

            try
            {
                var devToolsClient = browser.GetBrowser().GetDevToolsClient();
                
                var layoutMetricsResponse = await devToolsClient.Page.GetLayoutMetricsAsync();
                
                if (layoutMetricsResponse == null)
                {
                    MessageBox.Show("无法获取网页尺寸信息", "截图失败", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                
                var contentSize = layoutMetricsResponse.ContentSize;
                int totalWidth = (int)Math.Ceiling(contentSize.Width);
                int totalHeight = (int)Math.Ceiling(contentSize.Height);
                
                totalWidth = Math.Min(totalWidth, 10000);
                totalHeight = Math.Min(totalHeight, 15000);

                var viewport = new CefSharp.DevTools.Page.Viewport
                {
                    X = 0,
                    Y = 0,
                    Width = totalWidth,
                    Height = totalHeight,
                    Scale = dpiScale
                };

                Console.WriteLine($"截图尺寸: 宽度={totalWidth}, 高度={totalHeight}, 缩放比例={dpiScale}");

                var captureResponse = await devToolsClient.Page.CaptureScreenshotAsync(
                    CefSharp.DevTools.Page.CaptureScreenshotFormat.Png, 
                    100,
                    viewport,
                    true,
                    true
                );
                
                if (captureResponse?.Data == null)
                {
                    return null;
                }
                
                byte[] imageBytes = captureResponse.Data;
                
                using (var ms = new MemoryStream(imageBytes))
                {
                    return new Bitmap(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"截图过程中出错: {ex.Message}", "截图错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            browser?.Dispose();
            base.OnFormClosing(e);
        }

        // 自定义上下文菜单处理程序
        private class CustomContextMenuHandler : IContextMenuHandler
        {
            private readonly MainForm _mainForm;

            public CustomContextMenuHandler(MainForm mainForm)
            {
                _mainForm = mainForm;
            }

            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                // 清除默认的"在新窗口打开"和"在新标签页打开"选项
                if (model.Count > 0)
                {
                    // 移除"在新窗口中打开链接"选项
                    if (parameters.LinkUrl != null && parameters.LinkUrl != "")
                    {
                        // 使用正确的枚举值
                        // CefSharp中常用的菜单命令ID值
                        const int OpenInNewWindow = 34;
                        const int OpenInNewTab = 35;
                        
                        model.Remove((CefMenuCommand)OpenInNewWindow);
                        model.Remove((CefMenuCommand)OpenInNewTab);
                    }
                }
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                // 如果是链接相关命令，在当前窗口打开
                if (parameters.LinkUrl != null && parameters.LinkUrl != "")
                {
                    // 默认的打开链接命令
                    const int OpenLink = 33;
                    
                    if (commandId == (CefMenuCommand)OpenLink)
                    {
                        _mainForm.NavigateToUrl(parameters.LinkUrl);
                        return true;
                    }
                }
                
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
                // 不需要特殊处理
            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false; // 返回false使用默认实现
            }
        }
    }
}
