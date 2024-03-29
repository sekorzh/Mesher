﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesher.Core.Events;
using Mesher.Core.Events.EventArgs;
using Mesher.Core.Plugins;
using Mesher.Graphics;

namespace Mesher.Core
{
    public class MesherApplication : IDisposable
    {
        private Document m_currentDocument;
        public MesherGraphics Graphics { get; set; }

        public Document CurrentDocument
        {
            get { return m_currentDocument; }
            set
            {
                var documentChangeEventArgs = new ChangeDocumentEventArgs();
                BeforeDocumentChange?.Invoke(this, documentChangeEventArgs);
                m_currentDocument = value;
                AfterDocumentChange?.Invoke(this, documentChangeEventArgs);
            }
        }

        public MainWindow MainWindow { get; private set; }

        public List<IDocumentView> DocumentViews { get; private set; }

        public PluginSystem PluginsSystem { get; private set; }

        public event OnBeforeDocumentChange BeforeDocumentChange;
        public event OnAfterDocumentChange AfterDocumentChange;
        public event OnDocumentViewAdd DocumentViewAdd;
        public event OnDocumentViewRemove DocumentViewRemove;

        public MesherApplication()
        {
            PluginsSystem = new PluginSystem(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar);

            Graphics = new GlWindowsGraphics();
            MainWindow = new MainWindow(this);
            DocumentViews = new List<IDocumentView> {MainWindow.DocumentView};
        }

        public Document LoadDocument(String fileName)
        {
            return Document.Load(fileName, this);
        }

        public void Dispose()
        {
            
        }
    }
}
