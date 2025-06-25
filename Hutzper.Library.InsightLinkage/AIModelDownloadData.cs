namespace Hutzper.Library.InsightLinkage
{
    [Serializable]
    public record Project
    {
        public int id { get; init; }
        public string uuid { get; init; } = string.Empty;
        public int company_id { get; init; }
        public int user_id { get; init; }
        public int ai_project_type_id { get; init; } // 1: 画像分類, 2: 異常検知, 3: セグメン, 4: 物体検出
        public DateTime created_at { get; init; }
        public DateTime updated_at { get; init; }
        public int label_studio_project_id { get; init; }
        public DateTime? deleted_at { get; init; }
        public string title { get; init; } = string.Empty;
        public string description { get; init; } = string.Empty;
        public string thumbnail { get; init; } = string.Empty;
    }
    [Serializable]
    public sealed record Model
    {
        static public string GetFileName(Model? item) => $"../onnx/{item?.ai_project_id}_{item?.inner_id}_{item?.uuid}.onnx";
        public int id { get; init; }
        public Dictionary<string, string>? args { get; init; }
        public Metrics? metrics { get; init; }
        public string status { get; init; } = string.Empty;
        public string training_job_arn { get; init; } = string.Empty;
        public string uuid { get; init; } = string.Empty;
        public int ai_algorithm_id { get; init; }
        public int ai_project_dataset_id { get; init; }
        public int user_id { get; init; }
        public DateTime created_at { get; init; }
        public DateTime updated_at { get; init; }
        public int inner_id { get; init; }
        public string training_time { get; init; } = string.Empty;
        public int ai_project_id { get; init; }
        public string dataset_name { get; init; } = string.Empty;
    }

    [Serializable]
    public class Metrics
    {
        public double test_mIoU { get; init; }
        public double test_mIoU_except_background { get; init; }
        public double test_accuracy { get; init; }
        public double test_recall { get; init; }
        public double test_precision { get; init; }
    }
}