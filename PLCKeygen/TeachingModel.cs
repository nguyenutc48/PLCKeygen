using System;
using System.Collections.Generic;

namespace PLCKeygen
{
    /// <summary>
    /// Represents a teaching model with only ID and Name
    /// Coordinates are stored directly in PLC, not in this file
    /// Each port (1-4) has its own set of models (ID 1-100)
    /// </summary>
    public class TeachingModel
    {
        public int PortNumber { get; set; }  // Port 1-4
        public int ModelID { get; set; }  // ID from 1 to 100
        public string ModelName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string Description { get; set; }

        public TeachingModel()
        {
            PortNumber = 1;
            ModelID = 1;
            ModelName = string.Empty;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            Description = string.Empty;
        }

        public TeachingModel(int portNumber, int modelID, string modelName)
        {
            PortNumber = portNumber;
            ModelID = modelID;
            ModelName = modelName;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            Description = string.Empty;
        }
    }

    /// <summary>
    /// Container for all saved models
    /// </summary>
    public class TeachingModelCollection
    {
        public List<TeachingModel> Models { get; set; }

        public TeachingModelCollection()
        {
            Models = new List<TeachingModel>();
        }

        /// <summary>
        /// Find model by port and ID
        /// </summary>
        public TeachingModel FindModelByPortAndID(int portNumber, int modelID)
        {
            return Models.Find(m => m.PortNumber == portNumber && m.ModelID == modelID);
        }

        /// <summary>
        /// Find model by ID (deprecated - use FindModelByPortAndID)
        /// </summary>
        public TeachingModel FindModelByID(int modelID)
        {
            return Models.Find(m => m.ModelID == modelID);
        }

        /// <summary>
        /// Find model by name
        /// </summary>
        public TeachingModel FindModel(string modelName)
        {
            return Models.Find(m => m.ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Check if model ID exists for specific port
        /// </summary>
        public bool ModelIDExists(int portNumber, int modelID)
        {
            return Models.Exists(m => m.PortNumber == portNumber && m.ModelID == modelID);
        }

        /// <summary>
        /// Check if model name already exists
        /// </summary>
        public bool ModelExists(string modelName)
        {
            return Models.Exists(m => m.ModelName.Equals(modelName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all models for a specific port
        /// </summary>
        public List<TeachingModel> GetModelsForPort(int portNumber)
        {
            return Models.FindAll(m => m.PortNumber == portNumber);
        }

        /// <summary>
        /// Add new model
        /// </summary>
        public void AddModel(TeachingModel model)
        {
            if (ModelExists(model.ModelName))
            {
                throw new InvalidOperationException($"Model '{model.ModelName}' already exists.");
            }
            Models.Add(model);
        }

        /// <summary>
        /// Update existing model
        /// </summary>
        public void UpdateModel(TeachingModel model)
        {
            var existing = FindModel(model.ModelName);
            if (existing == null)
            {
                throw new InvalidOperationException($"Model '{model.ModelName}' not found.");
            }

            model.LastModified = DateTime.Now;
            int index = Models.IndexOf(existing);
            Models[index] = model;
        }

        /// <summary>
        /// Delete model by name
        /// </summary>
        public bool DeleteModel(string modelName)
        {
            var model = FindModel(modelName);
            if (model != null)
            {
                Models.Remove(model);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all model names
        /// </summary>
        public List<string> GetModelNames()
        {
            return Models.ConvertAll(m => m.ModelName);
        }
    }
}
