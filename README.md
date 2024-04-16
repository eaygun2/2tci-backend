# Adding a New Machine Learning Model to the Backend
In this guide, we will walk through the process of adding a damage detection model to our project backend. It is crucial to save the model in an ONNX (Open Neural Network Exchange) format, which is used for model interchange between different deep learning frameworks. ONNX files preserve the model's structure, including architecture, weights, and other parameters.

## Step 1: Studying the Model
Start by visiting https://netron.app. Select the desired model and examine the following information once it's loaded:
- Input/Vector Type: This indicates the required image dimensions to feed into the model. You can find this under the "input" section of the tensor. In this case, the required dimensions are 224 x 224 x 3. The term 'three' refers to the number of color channels, representing RGB (Red, Green, Blue).
- Output: This is the result returned by the model after making a prediction.

## Step 2: Adding Model Specifications to the Project
First, add the model specifications to the appsettings.json file. This includes specifying the path to the model, the required image dimensions, and the input/output column names of the model. Here is an example:

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

Now, navigate to the ModelBaseSettings.cs file. Add a new variable of type ModelSettings and make it nullable. The result should look like this:
```csharp
public class ModelSettingsBase
{
    public ModelSettings? ImageClassification { get; set; }
    public ModelSettings? CarDamageObjectDetection { get; set; }
}
```
This approach is used because the model service is generic. Instead of creating four separate classes differing only in model settings, a single class is used to fetch all settings from appsettings. Based on an enum specified in the request body, the correct settings are then determined dynamically.

Next, add a new model type to the enum under the ModelInputDto.cs file. This type will later be used to set the correct settings in the model service, fetched from appsettings.json. Make sure the name of the enum is the same as the entity class and appsettings section.

```csharp
public enum ModelType
{
    Undefined = 0,
    ImageClassification = 1,
    CarDamageObjectDetection = 2,
}
```

Then, go to the ModelService.cs file and locate the GetModelSettings method. Add the following to the switch statement:
```csharp
ModelType.CarObjectDetection => _modelSettingsBase.CarDamageObjectDetection!
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
Great, now all settings are configured.

## Step 3: Creating Necessary Entities
Each model has different input and output requirements. Therefore, it's essential to create a separate input class for each model. If multiple models serve the same purpose (e.g., both are object detection models), a single output class can be used for multiple models.

Begin by creating a new entity named CarDamageObjectDetectionModelInput. This entity should implement the IModelInput interface, as the model service only accepts entities that inherit this interface.
```csharp
public class CarDamageObjectDetectionModelInput : IModelInput
{
   // Change this to what has been entered in the appsettings.json.
   [ColumnName("input_2")]
   [VectorType(224 * 224 * 3)]
   public float[]? ImageData { get; set; }
}
```

Next, create an output class to store the image used for model inference.
```csharp
public class ModelOutputDto : Entity, ImodelOutput {

    [ColumnName("dense_5")]
    [VectorType(1)]
    [NotMapped]
    public float[]? Predictions { get; set; }
 
    public string? ImageBase64String { get; set; }
}
```

Finally, go to the Program.cs file and add the following under settings retrieval and update the database:
```csharp
builder.Services.AddSingleton<IModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>, ModelService<CarDamageObjectDetectionModelInput, ModelOutputDto>>();
```

## Step 4: Using the Service in the Controller
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
- Use it on the existing endpoint with conditional logic to save computing power.
- Create a new endpoint if the model doesn't fit within the current models' architecture.

## Step 5: Adding Tests
Finally, ensure the following are tested:
- The generic repository if a new output entity is created. Tests can be made generic, but this isn't necessary as one repository example's functionality should suffice.
- The service should be tested with the newly created entities, focusing particularly on the input.
- The controller must be tested with the new implementations to catch all potential errors. In most cases, this requires only one new test unless a new endpoint is added.
