using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Vision;
using PergunteAoPastorML.Values;

var rootFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;

const string assetsFolderName = "Assets";

const string hasPerguntaAssetsFolderName = "HasPerguntaData";
var hasPerguntaAssetsFolderPath = Path.Join(rootFolder, assetsFolderName, hasPerguntaAssetsFolderName);

const string doesNotHavePerguntaAssetsFolderName = "DoesNotHavePerguntaData";
var doesNotHavePerguntaAssetsFolderPath = Path.Join(rootFolder, assetsFolderName, doesNotHavePerguntaAssetsFolderName);

var hasPerguntaImageTrainingData = GetHasPerguntaTrainingImageData();
var doesNotHavePerguntaTrainingImageData = GetDoesNotHavePerguntaTrainingImageData();

var trainingSet = new List<ImageData>();
trainingSet.AddRange(hasPerguntaImageTrainingData);
trainingSet.AddRange(doesNotHavePerguntaTrainingImageData);

var mlContext = new MLContext();

var imagesData = mlContext.Data.LoadFromEnumerable(trainingSet);

var shuffledImagesData = mlContext.Data.ShuffleRows(imagesData);

var preprocessingPipeline = mlContext.Transforms.Conversion
    .MapValueToKey(
        inputColumnName: "Label",
        outputColumnName: "LabelKey")
    .Append(
        mlContext.Transforms.LoadRawImageBytes(
            outputColumnName: "ImageBytes",
            imageFolder: hasPerguntaAssetsFolderPath,
            inputColumnName: "ImagePath"))
    .Append(
        mlContext.Transforms.LoadRawImageBytes(
            outputColumnName: "ImageBytes",
            imageFolder: doesNotHavePerguntaAssetsFolderPath,
            inputColumnName: "ImagePath"));

var preProcessedData = preprocessingPipeline.Fit(shuffledImagesData).Transform(shuffledImagesData);

var trainSplit = mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.3);
var validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet, 0.5);

var trainSet = trainSplit.TrainSet;
var validationSet = validationTestSplit.TrainSet;
var testSet = validationTestSplit.TestSet;

var classifierOptions = new ImageClassificationTrainer.Options()
{
    //input column for the model
    FeatureColumnName = "ImageBytes",
    //target variable column 
    LabelColumnName = "LabelKey",
    //IDataView containing validation set
    ValidationSet = validationSet,
    //define pretrained model to be used
    Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
    //track progress during model training
    MetricsCallback = (metrics) => Console.WriteLine(metrics),
    /*if TestOnTrainSet is set to true, model is evaluated against
      Training set if validation set is not there*/
    TestOnTrainSet = false,
    //whether to use cached bottleneck values in further runs
    ReuseTrainSetBottleneckCachedValues = true,
    /*similar to ReuseTrainSetBottleneckCachedValues but for validation
      set instead of train set*/
    ReuseValidationSetBottleneckCachedValues = true,
};

var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

var trainedModel = trainingPipeline.Fit(trainSet);

SaveModel(mlContext, trainSet);

void SaveModel(MLContext mlContextToSave, IDataView trainingDataView)
{
    var outputModelsFolderName = "OutputModels";
    var pathToOutputModelsFolderName = Path.Join(rootFolder, outputModelsFolderName);

    if (!Directory.Exists(pathToOutputModelsFolderName)) Directory.CreateDirectory(pathToOutputModelsFolderName);

    mlContextToSave.Model.Save(trainedModel, trainingDataView.Schema,
        Path.Combine(pathToOutputModelsFolderName, $"model{DateTime.Today:yyyyMMdd}.zip"));
}


// ClassifyOneImg(mlContext, testSet, trainedModel);

static void ClassifyOneImg(MLContext myContext, IDataView data, ITransformer trainedModel)
{
    var predEngine = myContext.Model.CreatePredictionEngine<InputData, OutputData>(trainedModel);
    var images = myContext.Data.CreateEnumerable<InputData>(data, reuseRowObject: true);

    foreach (var image in images)
    {
        var prediction = predEngine.Predict(image);
        OutputPred(prediction);
    }
}

static void OutputPred(OutputData pred)
{
    string imgName = Path.GetFileName(pred.ImagePath);
    Console.WriteLine(
        $"Image: {imgName} | Actual Label: {pred.Label} | Predicted Label: {pred.PredictedLabel} | {(pred.Label == pred.PredictedLabel ? "Acertou" : "Errou")}");
}

List<ImageData> GetHasPerguntaTrainingImageData()
{
    var imageFiles = Directory.GetFiles(hasPerguntaAssetsFolderPath);
    var trainingData = new List<ImageData>();

    foreach (var imageFile in imageFiles)
    {
        trainingData.Add(new()
        {
            ImagePath = imageFile,
            Label = "tem pergunta"
        });
    }

    return trainingData;
}

List<ImageData> GetDoesNotHavePerguntaTrainingImageData()
{
    var imageFiles = Directory.GetFiles(doesNotHavePerguntaAssetsFolderPath);
    var trainingData = new List<ImageData>();
    foreach (var imageFile in imageFiles)
    {
        trainingData.Add(new()
        {
            ImagePath = imageFile,
            Label = "sem pergunta"
        });
    }

    return trainingData;
}