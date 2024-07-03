# Explaining the project architecture with an example case
In this guide, we will explain the architecture of this project by walking through the process of adding an object detection model to the backend. It's essential that the model is saved in the ONNX (Open Neural Network Exchange) format, which facilitates model interchange between different deep learning frameworks. ONNX files maintain the model's structure, including architecture, weights, and other parameters.

Before proceeding further, here are a few recommendations:

- [ ] Implement the Factory Pattern: This allows for the use of models in different formats, beyond just ONNX.
- [ ] Optimize Model Loading in ModelService: Currently, the model needs to be loaded each time it is used. To save processing time, implement a method to ensure the model is loaded only once.
- [ ] Consider using microservices: This allows that not all models, and thus the whole validation of vehicles, cannot be used if there is a sudden crash.

## Explanation of Why an Onion Architecture is Used
The onion architecture is chosen for several key reasons. Firstly, it enhances the testability and maintainability of the software. This architecture emphasizes the separation of concerns, allowing for easier modifications to frameworks or technologies without impacting the core domain. For example, it enables seamless transitions from SQL to MongoDB or from .NET to microservices. This flexibility ensures that the core functionality remains stable and unaffected by changes in external dependencies.

## Step 1: Add the Model File to the Project
Begin by adding the model file to the project. Ideally, this file should be saved in the assets folder located in the portal project (2TCI_Backend). This organization helps maintain a clear structure and ensures that the model file is easily accessible for the backend processes.

## Step 2: Adding Model Specifications to the Project
Now that we have the model imported, it is neccessary to make it useful. This part of the process required a few steps:
* Updating the appsettings.json file
* Updating the model settings classes
* Updating the model service and controller

### Updating appsettings.json
First, update the appsettings.json file to include specific model configurations. Centralizing model-related details within this configuration file is essential because each model has unique requirements. This centralization simplifies the retrieval and management of model settings, making maintenance and modifications more straightforward.

Add your specifications to the ModelSettings section of appsettings.json:

```json
  "ModelSettings": {
    "LicensePlateObjectDetection": {
      "ModelPath": "Assets/license-plate-detection_model.onnx",
    },
    "CarObjectDetection": {
      "ModelPath": "Assets/vehicle-detection_model.onnx",
    }
  }
```

In this configuration:
- ModelPath: Specifies the path to the ONNX model file.

Using appsettings.json to store these details ensures that model settings are easily accessible and can be updated without modifying the code. This approach enhances the flexibility and maintainability of the application. Additionally, if you need to add other parameters in the future, this centralized configuration method will make it easy to incorporate and manage them effectively. The [Netron tool](https://netron.app) could be of great help.

### Updating ModelBaseSettings.cs
Next, update the ModelBaseSettings.cs file to accommodate the newly added model configurations. This step involves introducing nullable variables of type ModelSettings within the ModelBaseSettings class. Here's how you can do it:
```csharp
public class ModelSettingsBase
{
    public ModelSettings? ImageClassification { get; set; }
    public ModelSettings? CarObjectDetection { get; set; }
    public ModelSettings? LicensePlateObjectDetection { get; set; }
}
```

This class gives us the following advantages:
- Single Configuration Class: Instead of maintaining multiple classes with similar properties, a single ModelBaseSettings class consolidates all model settings within a unified structure.
- Dynamic Settings Retrieval: Using an enum specified in the request body, such as ModelType, the application can dynamically determine and fetch the appropriate model settings from the program.cs file.

The settings are being picked up from the program.cs file:
```csharp
builder.Services.Configure<ModelSettingsBase>(builder.Configuration.GetSection("ModelSettings"));
```

And this is what the ModelSettings Class looks like:
```csharp
// Settings for one model
public class ModelSettings
{
    public string? ModelPath { get; set; }
}
```

It's important to note that the ModelSettings class must have the same variables as those defined in the appsettings.json file. Keeping this structure consistent ensures that the class will correctly receive and utilize the configuration values.

### Adding a ModelType
Next, add a new model type to the enum under the ModelInputDto.cs file. This enum will play a crucial role in setting the correct model configurations within the model service, based on the ModelType specified.

```csharp
public enum ModelType
{
    Undefined = 0,
    CarObjectDetection = 1,
    LicensePlateObjectDetection = 2,
}
```

#### Integrating ModelType enum in ModelService.cs
Next, navigate to the ModelService.cs file and locate the GetModelSettings method. Update the switch statement to handle different ModelType values and retrieve the corresponding model settings from ModelBaseSettings.
```csharp
ModelType.CarDamageObjectDetection => _modelSettingsBase.CarDamageObjectDetection!
```

This should result in:
```csharp
private ModelSettings GetModelSettings(ModelType type)
{
// For new model, add enum here
return type switch
    {
        ModelType.CarObjectDetection => _modelSettingsBase.CarObjectDetection!,
        ModelType.LicensePlateObjectDetection => _modelSettingsBase.LicensePlateObjectDetection!,
        ModelType.Undefined => throw new ArgumentNullException(nameof(ModelType), $"Unsupported input type: {type}"),
    };
}
```

By integrating the ModelType enum and updating the GetModelSettings method, your application is equipped to dynamically manage and apply model configurations based on user-defined model types, enhancing adaptability and robustness in the backend system configuration process.

## Step 3: Creating Necessary Entities
Machine learning models often have specific requirements for input and output data. Therefore, it's crucial to design dedicated input and output classes tailored to each model's needs. If multiple models share similar functionalities, a unified output class can be utilized.

### Creating An Input Class
Begin by defining a new entity if needed which implements the IModelInput interface. This interface is essential as the model service exclusively accepts entities that inherit this interface.
```csharp
public class ModelInputDto : IModelInput {
    [Required]
    public string? ImageBase64String { get; set; }

    [Required]
    public ModelType ModelType { get; set; } = ModelType.Undefined;
}
```

### Creating An Output Class
Next, define an output class to store the results of model inference, including prediction outcomes and associated image data. Again, if multiple models share similar functionalities, a unified output class can be utilized.
```csharp
public class DetectionModelOutput : EntityBase, IModelOutput
{
    public string? ImageBase64String { get; set; }
    public float[]? Box { get; set; }
    public string? Class { get; set; }
    public float? Score { get; set; }
}
```

The reason for inheriting from EntityBase is to facilitate the storage of the model in the database later on. This class solely includes an Id property.
```csharp
public class EntityBase
{
    [Key]
    public int Id { get; set; }
}
```

To save it in the database, add a DbSet variable to the DBContext class located in the infrastructure project. This variable represents a table in the database.
```csharp
public DbSet<DetectionModelOutput> CarObjectDetectionPredictions { get; set; }
```

To migrate using Developer PowerShell, execute the following commands:
**NOTE: Update the database reference in appsettings.json before executing the commands** 

```
dotnet ef migrations add Detection --project Infrastructure --startup-project 2TCI_Backend
dotnet ef database update --project Infrastructure --startup-project 2TCI_Backend
```

### Registering Model Service in Program.cs
Finally, integrate the model service with dependency injection in the application's Program.cs file to enable the usage of the defined input and output entities. Only perform this integration for each specific technology, such as classification and object detection, unless otherwise necessary.
```csharp
builder.Services.AddSingleton<IModelService<ModelInputDto, DetectionModelOutput>, ModelService<ModelInputDto, DetectionModelOutput>>();
```

By following these steps, you establish a structured approach to handling input and output data for machine learning models within your backend system, ensuring compatibility and efficiency in model integration and utilization. Adjust these implementations as per your specific project requirements and model configurations.

## Step 4: Using the Service in the Controller
To integrate the machine learning model service into your controller, follow these steps:

### Dependency Injection Setup in Controller
Register the service and repository as an interface via dependency injection. Only output repositories are created. Add the following:
```csharp
private readonly IModelService<ModelInputDto, DetectionModelOutput> _service;
private readonly IRepositoryBase<DetectionModelOutput> _repository;
 
public ModelController(IModelService<ModelInputDto, DetectionModelOutput> service, IRepositoryBase<DetectionModelOutput> repository)
{
    _service = service;
    _repository = repository;
}
```
Now, choose one of the two options to use the service in the endpoint:
1. Use it on the existing endpoint with conditional logic to save computing power.
2. Create a new endpoint if the model doesn't fit within the current endpoint.

## Step 5: Adding Tests
These tests are designed to verify the functionality and reliability of the integrated model components. Nsubstitute is utilized for mocking, which simplifies the testing process. While adding new tests specifically for adding a model is not required, we encourage you to consider adding or modifying tests whenever you make changes to the code. This ensures thorough validation of the system's behavior and enhances overall code quality.

### Purpose of Repository Testing
- Data Persistence: Validate that the repository correctly handles data operations (e.g., saving, retrieving, updating) for the ModelOutputDto entity.
- Error Handling: Ensure robust error handling within repository methods to handle various scenarios (e.g., data not found).

### Purpose of Service Testing
- Input Validation: Validate that the model service correctly handles input data (e.g., image data) and processes it according to the model's requirements.
- Prediction Accuracy: Verify that the service produces accurate predictions based on input data, reflecting the expected behavior of the machine learning model.
  
### Testing the Controller
Modify the tests to test the new models.

#### Purpose of Controller Testing
- Endpoint Functionality: Validate that controller actions correctly handle incoming requests and invoke the appropriate model service methods based on request parameters (e.g., model type).
- Error Handling and Response Format: Ensure that the controller returns appropriate responses (e.g., success or error messages, HTTP status codes) based on the outcome of model predictions and data processing.

#### Example Test Scenarios
##### Repository Test:
```csharp
[Fact]
public async Task Create_Should_Add_Entity_When_Valid_Entity_Is_Given()
{
    //Arrange
    var entity = new DetectionModelOutput()!;

    //Act
    await _sut.AddAsync(entity);

    //Assert
    await _sut.Received(1).AddAsync(Arg.Any<DetectionModelOutput>());
}
```
##### Service Test:
```csharp
Copy code
[Fact]
public void Constructor_Should_ThrowArgumentNullException_When_ModelSettingsBase_IsNull()
{
    // Arrange
    _modelSettingsBase.Value.Returns((ModelSettingsBase)null);

    // Act & Assert
    var exception = Assert.Throws<ArgumentNullException>(() => new ModelService<ModelInputDto, DetectionModelOutput>(_modelSettingsBase));
    Assert.Equal("ModelSettingsBase", exception.ParamName);
}
```
##### Controller Test:
```csharp
[Fact]
public async Task Predict_Should_Return_BadRequest_When_Input_Is_Null()
{
    // Arrange
    ModelInputDto input = null;

    // Act
    var result = await _sut.Predict(input);

    // Assert
    Assert.IsType<BadRequestResult>(result);

    await _service.DidNotReceive().Predict(Arg.Any<ModelInputDto>());
    await _repository.DidNotReceive().AddAsync(Arg.Any<DetectionModelOutput>());
}
```








