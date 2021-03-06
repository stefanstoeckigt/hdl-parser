using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Schematix.CommonProperties
{
    [Serializable()]
    public class Configuration :ISerializable
    {
        /// <summary>
        /// статическая переменная, хранящая текущую конфигурацию
        /// </summary>
        static Configuration conf = null;
        public static Configuration CurrentConfiguration
        {
            get
            {
                return conf;
            }
        }


        /// <summary>
        /// Путь к файлу с сохраненными размерами и положением панелей
        /// </summary>
        public string LayoutPath
        {
            get { return System.IO.Path.Combine(Schematix.CommonProperties.Configuration.CurrentConfiguration.ProjectOptions.DefaultProjectLocation, "Layout.config"); }
        }
        
        static Configuration()
        {
            conf = new Configuration();
            conf.LoadData();
        }
        
        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Configuration(SerializationInfo info, StreamingContext ctxt)
        {
            projectOptions =    (ProjectOptions)info.GetValue("ProjectOptions", typeof(ProjectOptions));
            textEditorOptions = (TextEditorOptions)info.GetValue("TextEditorOptions", typeof(TextEditorOptions));
            librariesOptions =  (LibrariesOptions)info.GetValue("LibrariesOptions", typeof(LibrariesOptions));
            _FSMOptions =       (FSMOptions)info.GetValue("FSMOptions", typeof(FSMOptions));
            _GHDLOptions = (GHDLOptions)info.GetValue("GHDLOptions", typeof(GHDLOptions));
            entityDrawningOptions = (EntityDrawningOptions)info.GetValue("EntityDrawningOptions", typeof(EntityDrawningOptions));            
        }

        private Configuration() { }

        /// <summary>
        /// Получение пути к конфигурационному файлу пользователя
        /// </summary>
        public static string ConfigurationPath
        {
            get
            {
                return 
                    System.IO.Path.Combine
                    (
                        System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "HDL_Light\\config.conf"
                    );
            }
        }

        /// <summary>
        /// Свойства проекта
        /// </summary>
        private ProjectOptions projectOptions;
        public ProjectOptions ProjectOptions
        {
            get
            {
                return projectOptions;
            }
        }

        /// <summary>
        /// Свойства текстового редактора
        /// </summary>
        private TextEditorOptions textEditorOptions;
        public TextEditorOptions TextEditorOptions
        {
            get
            {
                return textEditorOptions;
            }
        }

        /// <summary>
        /// Свойства библиотеки
        /// </summary>
        private LibrariesOptions librariesOptions;
        public LibrariesOptions LibrariesOptions
        {
            get
            {
                return librariesOptions;
            }
        }
        /// <summary>
        /// Свойства FSM
        /// </summary>
        private FSMOptions _FSMOptions;
        public FSMOptions FSMOptions
        {
            get
            {
                return _FSMOptions;
            }
        }

        /// <summary>
        /// Свойства для EntityDrawning
        /// </summary>
        private EntityDrawningOptions entityDrawningOptions;
        public EntityDrawningOptions EntityDrawningOptions
        {
            get
            {
                return entityDrawningOptions;
            }
        }

        /// <summary>
        /// Настройки среды GHDL
        /// </summary>
        private GHDLOptions _GHDLOptions;
        public GHDLOptions GHDLOptions
        {
            get { return _GHDLOptions; }
        }
        

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private void LoadData()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(ConfigurationPath, FileMode.Open);
                IFormatter formatter = new BinaryFormatter();
                conf = (Configuration)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                if (stream != null)
                    stream.Close();

                Schematix.Core.Logger.Log.Error("Load Data exception.", ex);

                System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show("Load Data exception. Set default configuration?", "Configuration error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Question);
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    SetDefaultConfiguration();
                    //Создание соответствующей папки
                    Directory.CreateDirectory(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HDL_Light"));
                    SaveChanges();
                }
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Загрузка данных на форму
        /// </summary>
        /// <param name="options"></param>
        public void LoadDataToForm(Options options)
        {
            projectOptions.LoadData(options);
            textEditorOptions.LoadData(options);
            librariesOptions.LoadData(options);
            _FSMOptions.LoadData(options);
            entityDrawningOptions.LoadData(options);
            _GHDLOptions.LoadData(options);
        }

        /// <summary>
        /// Сохранение изменений в конфигурационнй файл
        /// </summary>
        public void SaveChanges(Options options)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(ConfigurationPath, FileMode.Create);
                projectOptions.Accept(options);
                textEditorOptions.Accept(options);
                librariesOptions.Accept(options);
                _FSMOptions.Accept(options);
                _GHDLOptions.Accept(options);
                entityDrawningOptions.Accept(options);

                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "SaveChanges exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Schematix.Core.Logger.Log.Error("SaveChanges exception.", ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        /// <summary>
        /// Сохранение изменений в конфигурационнй файл
        /// </summary>
        public void SaveChanges()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(ConfigurationPath, FileMode.Create);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "SaveChanges exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Schematix.Core.Logger.Log.Error("SaveChanges exception.", ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public void SetDefaultConfiguration()
        {
            projectOptions = new ProjectOptions();
            projectOptions.SetDefault();
            textEditorOptions = new TextEditorOptions();
            textEditorOptions.SetDefault();
            librariesOptions = new LibrariesOptions();
            librariesOptions.SetDefault();
            _FSMOptions = new FSMOptions();
            _FSMOptions.SetDefault();
            entityDrawningOptions = new EntityDrawningOptions();
            entityDrawningOptions.SetDefault();
            _GHDLOptions = new GHDLOptions();
            _GHDLOptions.SetDefault();
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ProjectOptions", ProjectOptions);
            info.AddValue("TextEditorOptions", textEditorOptions);
            info.AddValue("LibrariesOptions", librariesOptions);
            info.AddValue("FSMOptions", FSMOptions);
            info.AddValue("GHDLOptions", GHDLOptions);
            info.AddValue("EntityDrawningOptions", EntityDrawningOptions);
        }

        #endregion
    }
}
