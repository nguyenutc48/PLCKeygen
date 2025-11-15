using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PLCKeygen
{
    /// <summary>
    /// Manager class for saving/loading teaching models to/from JSON files
    /// </summary>
    public class ModelManager
    {
        private static readonly string DEFAULT_MODELS_FOLDER = "TeachingModels";
        private static readonly string MODELS_FILE = "teaching_models.json";

        private string modelsFilePath;
        private TeachingModelCollection modelCollection;

        public ModelManager()
        {
            // Create models folder if it doesn't exist
            string modelsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_MODELS_FOLDER);
            if (!Directory.Exists(modelsFolder))
            {
                Directory.CreateDirectory(modelsFolder);
            }

            modelsFilePath = Path.Combine(modelsFolder, MODELS_FILE);
            modelCollection = LoadFromFile();
        }

        /// <summary>
        /// Get all models
        /// </summary>
        public TeachingModelCollection GetAllModels()
        {
            return modelCollection;
        }

        /// <summary>
        /// Get model by port and ID
        /// </summary>
        public TeachingModel GetModelByPortAndID(int portNumber, int modelID)
        {
            return modelCollection.FindModelByPortAndID(portNumber, modelID);
        }

        /// <summary>
        /// Get model by ID (deprecated)
        /// </summary>
        public TeachingModel GetModelByID(int modelID)
        {
            return modelCollection.FindModelByID(modelID);
        }

        /// <summary>
        /// Get model by name
        /// </summary>
        public TeachingModel GetModel(string modelName)
        {
            return modelCollection.FindModel(modelName);
        }

        /// <summary>
        /// Get or create model by port and ID with default name
        /// </summary>
        public TeachingModel GetOrCreateModelByPortAndID(int portNumber, int modelID)
        {
            var model = GetModelByPortAndID(portNumber, modelID);
            if (model == null)
            {
                // Create default model name
                string defaultName = $"Model {modelID}";
                model = new TeachingModel(portNumber, modelID, defaultName);
            }
            return model;
        }

        /// <summary>
        /// Get all models for a specific port
        /// </summary>
        public List<TeachingModel> GetModelsForPort(int portNumber)
        {
            return modelCollection.GetModelsForPort(portNumber);
        }

        /// <summary>
        /// Save new model
        /// </summary>
        public void SaveModel(TeachingModel model)
        {
            if (modelCollection.ModelExists(model.ModelName))
            {
                throw new InvalidOperationException($"Model '{model.ModelName}' đã tồn tại. Sử dụng UpdateModel để cập nhật.");
            }

            modelCollection.AddModel(model);
            SaveToFile();
        }

        /// <summary>
        /// Update existing model
        /// </summary>
        public void UpdateModel(TeachingModel model)
        {
            modelCollection.UpdateModel(model);
            SaveToFile();
        }

        /// <summary>
        /// Save or update model (auto-detect)
        /// </summary>
        public void SaveOrUpdateModel(TeachingModel model)
        {
            if (modelCollection.ModelExists(model.ModelName))
            {
                UpdateModel(model);
            }
            else
            {
                SaveModel(model);
            }
        }

        /// <summary>
        /// Delete model by name
        /// </summary>
        public bool DeleteModel(string modelName)
        {
            bool result = modelCollection.DeleteModel(modelName);
            if (result)
            {
                SaveToFile();
            }
            return result;
        }

        /// <summary>
        /// Check if model exists
        /// </summary>
        public bool ModelExists(string modelName)
        {
            return modelCollection.ModelExists(modelName);
        }

        /// <summary>
        /// Get all model names for ComboBox binding
        /// </summary>
        public string[] GetModelNames()
        {
            return modelCollection.GetModelNames().ToArray();
        }

        /// <summary>
        /// Load models from JSON file
        /// </summary>
        private TeachingModelCollection LoadFromFile()
        {
            try
            {
                if (File.Exists(modelsFilePath))
                {
                    string json = File.ReadAllText(modelsFilePath);
                    var collection = JsonConvert.DeserializeObject<TeachingModelCollection>(json);
                    return collection ?? new TeachingModelCollection();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash - return empty collection
                System.Diagnostics.Debug.WriteLine($"Error loading models: {ex.Message}");
            }

            return new TeachingModelCollection();
        }

        /// <summary>
        /// Save models to JSON file
        /// </summary>
        private void SaveToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(modelCollection, Formatting.Indented);
                File.WriteAllText(modelsFilePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lưu models: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Export single model to separate JSON file
        /// </summary>
        public void ExportModel(string modelName, string filePath)
        {
            var model = GetModel(modelName);
            if (model == null)
            {
                throw new InvalidOperationException($"Model '{modelName}' không tồn tại.");
            }

            try
            {
                string json = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi export model: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Import model from JSON file
        /// </summary>
        public TeachingModel ImportModel(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File không tồn tại: {filePath}");
                }

                string json = File.ReadAllText(filePath);
                var model = JsonConvert.DeserializeObject<TeachingModel>(json);

                if (model == null)
                {
                    throw new InvalidOperationException("File JSON không hợp lệ.");
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi import model: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get models file path for backup purposes
        /// </summary>
        public string GetModelsFilePath()
        {
            return modelsFilePath;
        }

        /// <summary>
        /// Reload models from file (useful after external changes)
        /// </summary>
        public void Reload()
        {
            modelCollection = LoadFromFile();
        }

        /// <summary>
        /// Get total number of models
        /// </summary>
        public int GetModelCount()
        {
            return modelCollection.Models.Count;
        }
    }
}
