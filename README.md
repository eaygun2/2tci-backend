# Adding a New Machine Learning Model to the Backend
In this guide, we will walk through the process of adding a damage detection model to our project backend. It is crucial to save the model in an ONNX (Open Neural Network Exchange) format, which is used for model interchange between different deep learning frameworks. ONNX files preserve the model's structure, including architecture, weights, and other parameters.

## Step 1: Studying the Model
To start, use the tool Netron (https://netron.app), which is designed for visualizing neural network models. Choose your desired model and inspect the following details once it's loaded:
- Input/Vector Type: This specifies the necessary image dimensions required by the model. Look for this information in the "input" section of the tensor view. For example, the required dimensions might be 224 x 224 x 3, where 'three' denotes the three color channels (RGB - Red, Green, Blue).
- Output: This is the result returned by the model after making a prediction.

Understanding these specifics is essential for effectively integrating the model into your backend system.

## Step 2: Adding Model Specifications to the Project
### Updating appsettings.json
Next, update the appsettings.json file with specific model configurations. This approach centralizes model-related details within a configuration file, which is crucial because each model has unique requirements. Centralization simplifies the retrieval and management of model settings, making it easier to maintain and modify as needed.

Include the following specifications in the ModelSettings section of appsettings.json:

```json
"ModelSettings": {
    "ImageClassification": {
      "ModelPath": "Assets/efficientnet-b0_model.onnx",
      "ImageWidth": 224,
      "ImageHeight": 224,
      "InputColumnName": "input_1",
      "OutputColumnName": "dense_3"
    },
    "CarDamageObjectDetection": {
      "ModelPath": "Assets/efficientnet-b1_model.onnx",
      "ImageWidth": 224,
      "ImageHeight": 224,
      "InputColumnName": "input_2",
      "OutputColumnName": "dense_5"
    }
}
```

In this configuration:
- ModelPath: Specifies the path to the ONNX model file.
- ImageWidth and ImageHeight: Define the required dimensions of input images.
- InputColumnName and OutputColumnName: Identify the names of input and output columns used by the model.

Utilizing appsettings.json for storing these details ensures that model settings are easily accessible and can be updated without modifying code, enhancing the flexibility and maintainability of the application.

### Updating ModelBaseSettings.cs
Next, update the ModelBaseSettings.cs file to accommodate the newly added model configurations. This step involves introducing a nullable variable of type ModelSettings within the ModelBaseSettings class. Here's how you can do it:
```csharp
public class ModelSettingsBase
{
    public ModelSettings? ImageClassification { get; set; }
    public ModelSettings? CarDamageObjectDetection { get; set; }
}
```
#### Purpose of Using ModelBaseSettings
- Generic Model Service: This approach supports a generic model service that can handle multiple models without creating separate classes for each model's settings.
- Single Configuration Class: Instead of maintaining multiple classes with similar properties, a single ModelBaseSettings class consolidates all model settings within a unified structure.
- Dynamic Settings Retrieval: Using an enum specified in the request body, such as ModelType, the application can dynamically determine and fetch the appropriate model settings from appsettings.json.

By implementing ModelBaseSettings in this manner, your application gains flexibility and scalability in managing and accessing model configurations, supporting a more efficient and maintainable model integration process.

### Updating ModelType
Next, add a new model type to the enum under the ModelInputDto.cs file. This enum will play a crucial role in setting the correct model configurations within the model service, based on the ModelType specified.

```csharp
public enum ModelType
{
    Undefined = 0,
    ImageClassification = 1,
    CarDamageObjectDetection = 2,
}
```

#### Purpose of Using ModelType enum
- Dynamic Model Selection: The ModelType enum allows for dynamic selection of model configurations based on the specified model type.
- Consistency with Entity Class and AppSettings: Ensure that the enum name (ModelType) aligns with the entity class name (ModelInputDto) and the corresponding section in appsettings.json (ModelSettings). This consistency simplifies and standardizes the process of retrieving and applying model settings.

This structured approach enhances flexibility and maintainability, allowing for seamless integration of various machine learning models into the backend system.

#### Integrating ModelType enum in ModelService.cs
Next, navigate to the ModelService.cs file and locate the GetModelSettings method. Update the switch statement to handle different ModelType values and retrieve the corresponding model settings from ModelBaseSettings.
```csharp
ModelType.CarDamageObjectDetection => _modelSettingsBase.CarDamageObjectDetection!
```

This should result in:
```csharp
ModelSettings GetModelSettings(ModelType type)
{
  return type switch {
    ModelType.ImageClassification => _modelSettingsBase.ImageClassification!,
    ModelType.CarObjectDetection => _modelSettingsBase.CarDamageObjectDetecti   on!,
    ModelType.Undefined => throw new ArgumentNullException($"Unsupported input type: {type}"),
    };
}
```
#### Purpose of GetModelSettings Method
Dynamic Model Configuration Retrieval: The GetModelSettings method retrieves model settings based on the specified ModelType, ensuring that the correct configurations are applied to the corresponding machine learning model.
Error Handling: The method includes error handling to handle scenarios where model settings for a specific ModelType are not configured properly, preventing unexpected runtime issues.

By integrating the ModelType enum and updating the GetModelSettings method, your application is equipped to dynamically manage and apply model configurations based on user-defined model types, enhancing adaptability and robustness in the backend system configuration process.

## Step 3: Creating Necessary Entities
Machine learning models often have specific requirements for input and output data. Therefore, it's crucial to design dedicated input and output classes tailored to each model's needs. If multiple models share similar functionalities, a unified output class can be utilized.

### Creating input class: CarDamageObjectDetectionModelInput
Begin by defining a new entity named CarDamageObjectDetectionModelInput, which implements the IModelInput interface. This interface is essential as the model service exclusively accepts entities that inherit this interface.
```csharp
public class CarDamageObjectDetectionModelInput : IModelInput
{
   // Change this to what has been entered in the appsettings.json.
   [ColumnName("input_2")]
   [VectorType(224 * 224 * 3)]
   public float[]? ImageData { get; set; }
}
```

#### Purpose of CarDamageObjectDetectionModelInput Entity
- Interface Implementation: By implementing IModelInput, the CarDamageObjectDetectionModelInput class ensures compatibility with the model service, facilitating data input for model inference.
- ImageData Property: Represents the input image data required by the model, configured based on appsettings.json specifications.

### Creating Output Class: ModelOutputDto
Next, define an output class named ModelOutputDto to store the results of model inference, including prediction outcomes and associated image data.
```csharp
public class ModelOutputDto : Entity, IModelOutput {

    [ColumnName("dense_5")]
    [VectorType(1)]
    [NotMapped]
    public float[]? Predictions { get; set; }
 
    public string? ImageBase64String { get; set; }
}
```

#### Purpose of ModelOutputDto Entity
- Interface Implementation: By implementing IModelOutput, the ModelOutputDto class encapsulates model prediction results and associated image data.
- Predictions Property: Stores the model's output predictions.
- ImageBase64String Property: Represents the base64-encoded image data used for model inference and output visualization.

### Registering Model Service in Program.cs
Finally, integrate the model service with dependency injection in the application's Program.cs file to facilitate the usage of the defined input and output entities.
```csharp
builder.Services.AddSingleton<IModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>, ModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>>();
```

#### Purpose of Registering Model Service
- Dependency Injection: By registering the model service with CarDamageObjectDetectionModelInput and ModelOutputDto types, the application gains access to functionalities for utilizing these entities in model inference and backend operations.

By following these steps, you establish a structured approach to handling input and output data for machine learning models within your backend system, ensuring compatibility and efficiency in model integration and utilization. Adjust these implementations as per your specific project requirements and model configurations.

## Step 4: Using the Service in the Controller
To integrate the machine learning model service into your controller, follow these steps:
### Dependency Injection Setup in Controller
Register the service and repository as an interface via dependency injection. Only output repositories are created. Add the following:
```csharp
private readonly IModelService<CarDamageObjectDetectionModelInput, ModelOutputDto> _service;
private readonly IRepositoryBase<ModelOutputDto> _repository;
 
public ModelController(IModelService< CarDamageObjectDetectionModelInput, ModelOutputDto> service, IRepositoryBase<ModelOutputDto> repository)
{
    _service = service;
    _repository = repository;
}
```
Now, choose one of the two options to use the service in the endpoint:
- Use it on the existing endpoint with conditional logic to save computing power. This is because the current endpoint used is making use of multiple models, where each model fills in another model.
- Create a new endpoint if the model doesn't fit within the current models' architecture.

## Step 5: Adding Tests
Ensure thorough testing to validate the functionality and reliability of the newly integrated machine learning model components.

### 1. Testing the Generic Repository
If a new output entity (ModelOutputDto) is created, ensure to test it with the generic repository associated with it. While tests can be made generic, it's essential to verify specific functionalities related to the repository's operations.

#### Purpose of Repository Testing
- Data Persistence: Validate that the repository correctly handles data operations (e.g., saving, retrieving, updating) for the ModelOutputDto entity.
- Error Handling: Ensure robust error handling within repository methods to handle various scenarios (e.g., data not found, database connection issues).

### 2. Testing the Model Service
Test the model service (IModelService) with the newly created input entity (CarDamageObjectDetectionModelInput) to verify its functionality and accuracy in processing model predictions.

#### Purpose of Service Testing
- Input Validation: Validate that the model service correctly handles input data (e.g., image data) and processes it according to the model's requirements.
- Prediction Accuracy: Verify that the service produces accurate predictions based on input data, reflecting the expected behavior of the machine learning model.
  
### 3. Testing the Controller
Test the controller (ModelController) with the new implementations to identify and address potential errors or issues related to endpoint integrations and model service interactions.

#### Purpose of Controller Testing
- Endpoint Functionality: Validate that controller actions correctly handle incoming requests and invoke the appropriate model service methods based on request parameters (e.g., model type).
- Error Handling and Response Format: Ensure that the controller returns appropriate responses (e.g., success or error messages, HTTP status codes) based on the outcome of model predictions and data processing.
Considerations for Testing
- Unit Testing vs. Integration Testing: Implement both unit tests (isolated tests for individual components) and integration tests (testing interactions between components) to cover different aspects of functionality.
Mocking Dependencies: Use mocking frameworks to simulate dependencies (e.g., database access, external services) in tests, ensuring focused testing of specific components.

#### Example Test Scenarios
##### Repository Test:
```csharp
[Fact]
public void Repository_SaveModelOutput_ReturnsTrue()
{
    // Arrange
    var repository = new RepositoryBase<ModelOutputDto>();

    // Act
    var modelOutput = new ModelOutputDto { Predictions = new float[] { 0.8f, 0.2f } };
    var result = repository.Save(modelOutput);

    // Assert
    Assert.True(result);
}
```
##### Service Test:
```csharp
Copy code
[Fact]
public void ModelService_Predict_ValidInput_ReturnsPrediction()
{
    // Arrange
    var service = new ModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>();
    var modelInput = new CarDamageObjectDetectionModelInput { ImageData = new float[224 * 224 * 3] };

    // Act
    var prediction = service.Predict(modelInput);

    // Assert
    Assert.NotNull(prediction);
    Assert.NotNull(prediction.Predictions);
}
```
##### Controller Test:
```csharp
[Fact]
public void ModelController_Predict_ValidRequest_ReturnsOkResult()
{
    // Arrange
    var serviceMock = new Mock<IModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>>();
    var controller = new ModelController(serviceMock.Object, new RepositoryBase<ModelOutputDto>());
    var request = new PredictionRequestDto { ModelType = ModelType.CarDamageObjectDetection, ImageData = new float[224 * 224 * 3] };

    // Act
    var result = controller.Predict(request) as OkObjectResult;

    // Assert
    Assert.NotNull(result);
    Assert.Equal(200, result.StatusCode);
}
```

By implementing comprehensive tests for the generic repository, model service, and controller, you ensure the reliability and correctness of the machine learning model integration within your backend system. Adjust test scenarios based on specific requirements and edge cases to achieve thorough test coverage and robust application validation.






