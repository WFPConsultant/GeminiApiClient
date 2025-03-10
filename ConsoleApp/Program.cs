﻿/*
 * Copyright 2024 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START aiplatform_gemini_get_started]

using Google.Api.Gax.Grpc;
using Google.Cloud.AIPlatform.V1;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class Program
{
   
        static async Task Main(string[] args)
        {
            // You can pass the necessary parameters or use default values
            string projectId = "<your project id>";
            string location = "us-central1"; //"europe-west1";//
        string publisher = "google";
            string model = "gemini-1.0-pro-vision"; //"gemini-1.0-pro";//

        // Create an instance of the class containing the GenerateContent method
        YourClassContainingGenerateContent instance = new YourClassContainingGenerateContent();

            // Call the GenerateContent method
            string result = await instance.GenerateContent(projectId, location, publisher, model);

            // Print or use the result as needed
            Console.WriteLine(result);
        }

    public class YourClassContainingGenerateContent
    {
        public async Task<string> GenerateContent(
        string projectId,
        string location,
        string publisher,
        string model 
    )
        {
            // Create client
            var predictionServiceClient = new PredictionServiceClientBuilder
            {
                Endpoint = $"{location}-aiplatform.googleapis.com",
                CredentialsPath = "C:\\Practice\\ConsoleApp\\ConsoleApp\\account-secret.json" // Replace with the actual path
            }.Build();

            // Prompt
            string prompt = "What's in this photo";
            string imageUri = "gs://generativeai-downloads/images/scones.jpg";

            // Initialize request argument(s)
            var content = new Content
            {
                Role = "USER"
            };
            content.Parts.AddRange(new List<Part>()
        {
            new() {
                Text = prompt
            },
            new() {
                FileData = new() {
                    MimeType = "image/png",
                    FileUri = imageUri
                }
            }
        });

            var generateContentRequest = new GenerateContentRequest
            {
                Model = $"projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}",
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0.4f,
                    TopP = 1,
                    TopK = 32,
                    MaxOutputTokens = 2048
                }
            };
            generateContentRequest.Contents.Add(content);

            // Make the request, returning a streaming response
            using PredictionServiceClient.StreamGenerateContentStream response = predictionServiceClient.StreamGenerateContent(generateContentRequest);

            StringBuilder fullText = new();

            // Read streaming responses from server until complete
            AsyncResponseStream<GenerateContentResponse> responseStream = response.GetResponseStream();
            await foreach (GenerateContentResponse responseItem in responseStream)
            {
                fullText.Append(responseItem.Candidates[0].Content.Parts[0].Text);
            }

            return fullText.ToString();
        }
    }
}

// [END aiplatform_gemini_get_started]
