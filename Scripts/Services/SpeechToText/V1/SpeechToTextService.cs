/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System.Collections.Generic;
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.SpeechToText.V1.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.SpeechToText.V1
{
    public partial class SpeechToTextService : BaseService
    {
        private const string serviceId = "speech_to_text";
        private const string defaultUrl = "https://stream.watsonplatform.net/speech-to-text/api";

        #region Credentials
        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set
            {
                credentials = value;
                if (!string.IsNullOrEmpty(credentials.Url))
                {
                    Url = credentials.Url;
                }
            }
        }
        #endregion

        #region Url
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion

        #region VersionDate
        #endregion

        #region DisableSslVerification
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        /// <summary>
        /// SpeechToTextService constructor.
        /// </summary>
        
        public SpeechToTextService() : base(serviceId)
        {
            
        }

        /// <summary>
        /// SpeechToTextService constructor.
        /// </summary>
        
        /// <param name="credentials">The service credentials.</param>
        public SpeechToTextService(Credentials credentials) : base(credentials, serviceId)
        {
            if (credentials.HasCredentials() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = defaultUrl;
                }
            }
            else
            {
                throw new IBMException("Please provide a username and password or authorization token to use the SpeechToText service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Get a model.
        ///
        /// Gets information for a single specified language model that is available for use with the service. The
        /// information includes the name of the model and its minimum sampling rate in Hertz, among other things.
        ///
        /// **See also:** [Languages and models](https://cloud.ibm.com/docs/services/speech-to-text/models.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">The identifier of the model in the form of its name from the output of the **Get a
        /// model** method.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="SpeechModel" />SpeechModel</returns>
        public bool GetModel(Callback<SpeechModel> callback, string modelId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetModel`");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `GetModel`");

            RequestObject<SpeechModel> req = new RequestObject<SpeechModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/models/{0}", modelId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SpeechModel> response = new DetailedResponse<SpeechModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SpeechModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SpeechModel>)req).Callback != null)
                ((RequestObject<SpeechModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List models.
        ///
        /// Lists all language models that are available for use with the service. The information includes the name of
        /// the model and its minimum sampling rate in Hertz, among other things.
        ///
        /// **See also:** [Languages and models](https://cloud.ibm.com/docs/services/speech-to-text/models.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="SpeechModels" />SpeechModels</returns>
        public bool ListModels(Callback<SpeechModels> callback, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListModels`");

            RequestObject<SpeechModels> req = new RequestObject<SpeechModels>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/models");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SpeechModels> response = new DetailedResponse<SpeechModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SpeechModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SpeechModels>)req).Callback != null)
                ((RequestObject<SpeechModels>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Recognize audio.
        ///
        /// Sends audio and returns transcription results for a recognition request. You can pass a maximum of 100 MB
        /// and a minimum of 100 bytes of audio with a request. The service automatically detects the endianness of the
        /// incoming audio and, for audio that includes multiple channels, downmixes the audio to one-channel mono
        /// during transcoding. The method returns only final results; to enable interim results, use the WebSocket API.
        ///
        ///
        /// **See also:** [Making a basic HTTP
        /// request](https://cloud.ibm.com/docs/services/speech-to-text/http.html#HTTP-basic).
        ///
        /// ### Streaming mode
        ///
        ///  For requests to transcribe live audio as it becomes available, you must set the `Transfer-Encoding` header
        /// to `chunked` to use streaming mode. In streaming mode, the service closes the connection (status code 408)
        /// if it does not receive at least 15 seconds of audio (including silence) in any 30-second period. The service
        /// also closes the connection (status code 400) if it detects no speech for `inactivity_timeout` seconds of
        /// streaming audio; use the `inactivity_timeout` parameter to change the default of 30 seconds.
        ///
        /// **See also:**
        /// * [Audio transmission](https://cloud.ibm.com/docs/services/speech-to-text/input.html#transmission)
        /// * [Timeouts](https://cloud.ibm.com/docs/services/speech-to-text/input.html#timeouts)
        ///
        /// ### Audio formats (content types)
        ///
        ///  The service accepts audio in the following formats (MIME types).
        /// * For formats that are labeled **Required**, you must use the `Content-Type` header with the request to
        /// specify the format of the audio.
        /// * For all other formats, you can omit the `Content-Type` header or specify `application/octet-stream` with
        /// the header to have the service automatically detect the format of the audio. (With the `curl` command, you
        /// can specify either `"Content-Type:"` or `"Content-Type: application/octet-stream"`.)
        ///
        /// Where indicated, the format that you specify must include the sampling rate and can optionally include the
        /// number of channels and the endianness of the audio.
        /// * `audio/alaw` (**Required.** Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/basic` (**Required.** Use only with narrowband models.)
        /// * `audio/flac`
        /// * `audio/g729` (Use only with narrowband models.)
        /// * `audio/l16` (**Required.** Specify the sampling rate (`rate`) and optionally the number of channels
        /// (`channels`) and endianness (`endianness`) of the audio.)
        /// * `audio/mp3`
        /// * `audio/mpeg`
        /// * `audio/mulaw` (**Required.** Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/ogg` (The service automatically detects the codec of the input audio.)
        /// * `audio/ogg;codecs=opus`
        /// * `audio/ogg;codecs=vorbis`
        /// * `audio/wav` (Provide audio with a maximum of nine channels.)
        /// * `audio/webm` (The service automatically detects the codec of the input audio.)
        /// * `audio/webm;codecs=opus`
        /// * `audio/webm;codecs=vorbis`
        ///
        /// The sampling rate of the audio must match the sampling rate of the model for the recognition request: for
        /// broadband models, at least 16 kHz; for narrowband models, at least 8 kHz. If the sampling rate of the audio
        /// is higher than the minimum required rate, the service down-samples the audio to the appropriate rate. If the
        /// sampling rate of the audio is lower than the minimum required rate, the request fails.
        ///
        ///  **See also:** [Audio formats](https://cloud.ibm.com/docs/services/speech-to-text/audio-formats.html).
        ///
        /// ### Multipart speech recognition
        ///
        ///  **Note:** The Watson SDKs do not support multipart speech recognition.
        ///
        /// The HTTP `POST` method of the service also supports multipart speech recognition. With multipart requests,
        /// you pass all audio data as multipart form data. You specify some parameters as request headers and query
        /// parameters, but you pass JSON metadata as form data to control most aspects of the transcription.
        ///
        /// The multipart approach is intended for use with browsers for which JavaScript is disabled or when the
        /// parameters used with the request are greater than the 8 KB limit imposed by most HTTP servers and proxies.
        /// You can encounter this limit, for example, if you want to spot a very large number of keywords.
        ///
        /// **See also:** [Making a multipart HTTP
        /// request](https://cloud.ibm.com/docs/services/speech-to-text/http.html#HTTP-multi).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="audio">The audio to transcribe.</param>
        /// <param name="model">The identifier of the model that is to be used for the recognition request. See
        /// [Languages and models](https://cloud.ibm.com/docs/services/speech-to-text/models.html). (optional, default
        /// to en-US_BroadbandModel)</param>
        /// <param name="languageCustomizationId">The customization ID (GUID) of a custom language model that is to be
        /// used with the recognition request. The base model of the specified custom language model must match the
        /// model specified with the `model` parameter. You must make the request with credentials for the instance of
        /// the service that owns the custom model. By default, no custom language model is used. See [Custom
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input).
        ///
        /// **Note:** Use this parameter instead of the deprecated `customization_id` parameter. (optional)</param>
        /// <param name="acousticCustomizationId">The customization ID (GUID) of a custom acoustic model that is to be
        /// used with the recognition request. The base model of the specified custom acoustic model must match the
        /// model specified with the `model` parameter. You must make the request with credentials for the instance of
        /// the service that owns the custom model. By default, no custom acoustic model is used. See [Custom
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input). (optional)</param>
        /// <param name="baseModelVersion">The version of the specified base model that is to be used with recognition
        /// request. Multiple versions of a base model can exist when a model is updated for internal improvements. The
        /// parameter is intended primarily for use with custom models that have been upgraded for a new base model. The
        /// default value depends on whether the parameter is used with or without a custom model. See [Base model
        /// version](https://cloud.ibm.com/docs/services/speech-to-text/input.html#version). (optional)</param>
        /// <param name="customizationWeight">If you specify the customization ID (GUID) of a custom language model with
        /// the recognition request, the customization weight tells the service how much weight to give to words from
        /// the custom language model compared to those from the base model for the current request.
        ///
        /// Specify a value between 0.0 and 1.0. Unless a different customization weight was specified for the custom
        /// model when it was trained, the default value is 0.3. A customization weight that you specify overrides a
        /// weight that was specified when the custom model was trained.
        ///
        /// The default value yields the best performance in general. Assign a higher value if your audio makes frequent
        /// use of OOV words from the custom model. Use caution when setting the weight: a higher value can improve the
        /// accuracy of phrases from the custom model's domain, but it can negatively affect performance on non-domain
        /// phrases.
        ///
        /// See [Custom models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input).
        /// (optional)</param>
        /// <param name="inactivityTimeout">The time in seconds after which, if only silence (no speech) is detected in
        /// streaming audio, the connection is closed with a 400 error. The parameter is useful for stopping audio
        /// submission from a live microphone when a user simply walks away. Use `-1` for infinity. See [Inactivity
        /// timeout](https://cloud.ibm.com/docs/services/speech-to-text/input.html#timeouts-inactivity). (optional,
        /// default to 30)</param>
        /// <param name="keywords">An array of keyword strings to spot in the audio. Each keyword string can include one
        /// or more string tokens. Keywords are spotted only in the final results, not in interim hypotheses. If you
        /// specify any keywords, you must also specify a keywords threshold. You can spot a maximum of 1000 keywords.
        /// Omit the parameter or specify an empty array if you do not need to spot keywords. See [Keyword
        /// spotting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#keyword_spotting).
        /// (optional)</param>
        /// <param name="keywordsThreshold">A confidence value that is the lower bound for spotting a keyword. A word is
        /// considered to match a keyword if its confidence is greater than or equal to the threshold. Specify a
        /// probability between 0.0 and 1.0. If you specify a threshold, you must also specify one or more keywords. The
        /// service performs no keyword spotting if you omit either parameter. See [Keyword
        /// spotting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#keyword_spotting).
        /// (optional)</param>
        /// <param name="maxAlternatives">The maximum number of alternative transcripts that the service is to return.
        /// By default, the service returns a single transcript. If you specify a value of `0`, the service uses the
        /// default value, `1`. See [Maximum
        /// alternatives](https://cloud.ibm.com/docs/services/speech-to-text/output.html#max_alternatives). (optional,
        /// default to 1)</param>
        /// <param name="wordAlternativesThreshold">A confidence value that is the lower bound for identifying a
        /// hypothesis as a possible word alternative (also known as "Confusion Networks"). An alternative word is
        /// considered if its confidence is greater than or equal to the threshold. Specify a probability between 0.0
        /// and 1.0. By default, the service computes no alternative words. See [Word
        /// alternatives](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_alternatives).
        /// (optional)</param>
        /// <param name="wordConfidence">If `true`, the service returns a confidence measure in the range of 0.0 to 1.0
        /// for each word. By default, the service returns no word confidence scores. See [Word
        /// confidence](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_confidence). (optional,
        /// default to false)</param>
        /// <param name="timestamps">If `true`, the service returns time alignment for each word. By default, no
        /// timestamps are returned. See [Word
        /// timestamps](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_timestamps). (optional,
        /// default to false)</param>
        /// <param name="profanityFilter">If `true`, the service filters profanity from all output except for keyword
        /// results by replacing inappropriate words with a series of asterisks. Set the parameter to `false` to return
        /// results with no censoring. Applies to US English transcription only. See [Profanity
        /// filtering](https://cloud.ibm.com/docs/services/speech-to-text/output.html#profanity_filter). (optional,
        /// default to true)</param>
        /// <param name="smartFormatting">If `true`, the service converts dates, times, series of digits and numbers,
        /// phone numbers, currency values, and internet addresses into more readable, conventional representations in
        /// the final transcript of a recognition request. For US English, the service also converts certain keyword
        /// strings to punctuation symbols. By default, the service performs no smart formatting.
        ///
        /// **Note:** Applies to US English, Japanese, and Spanish transcription only.
        ///
        /// See [Smart formatting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#smart_formatting).
        /// (optional, default to false)</param>
        /// <param name="speakerLabels">If `true`, the response includes labels that identify which words were spoken by
        /// which participants in a multi-person exchange. By default, the service returns no speaker labels. Setting
        /// `speaker_labels` to `true` forces the `timestamps` parameter to be `true`, regardless of whether you specify
        /// `false` for the parameter.
        ///
        /// **Note:** Applies to US English, Japanese, and Spanish transcription only. To determine whether a language
        /// model supports speaker labels, you can also use the **Get a model** method and check that the attribute
        /// `speaker_labels` is set to `true`.
        ///
        /// See [Speaker labels](https://cloud.ibm.com/docs/services/speech-to-text/output.html#speaker_labels).
        /// (optional, default to false)</param>
        /// <param name="customizationId">**Deprecated.** Use the `language_customization_id` parameter to specify the
        /// customization ID (GUID) of a custom language model that is to be used with the recognition request. Do not
        /// specify both parameters with a request. (optional)</param>
        /// <param name="grammarName">The name of a grammar that is to be used with the recognition request. If you
        /// specify a grammar, you must also use the `language_customization_id` parameter to specify the name of the
        /// custom language model for which the grammar is defined. The service recognizes only strings that are
        /// recognized by the specified grammar; it does not recognize other custom words from the model's words
        /// resource. See [Grammars](https://cloud.ibm.com/docs/services/speech-to-text/input.html#grammars-input).
        /// (optional)</param>
        /// <param name="redaction">If `true`, the service redacts, or masks, numeric data from final transcripts. The
        /// feature redacts any number that has three or more consecutive digits by replacing each digit with an `X`
        /// character. It is intended to redact sensitive numeric data, such as credit card numbers. By default, the
        /// service performs no redaction.
        ///
        /// When you enable redaction, the service automatically enables smart formatting, regardless of whether you
        /// explicitly disable that feature. To ensure maximum security, the service also disables keyword spotting
        /// (ignores the `keywords` and `keywords_threshold` parameters) and returns only a single final transcript
        /// (forces the `max_alternatives` parameter to be `1`).
        ///
        /// **Note:** Applies to US English, Japanese, and Korean transcription only.
        ///
        /// See [Numeric redaction](https://cloud.ibm.com/docs/services/speech-to-text/output.html#redaction).
        /// (optional, default to false)</param>
        /// <param name="contentType">The format (MIME type) of the audio. For more information about specifying an
        /// audio format, see **Audio formats (content types)** in the method description. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="SpeechRecognitionResults" />SpeechRecognitionResults</returns>
        public bool Recognize(Callback<SpeechRecognitionResults> callback, byte[] audio, Dictionary<string, object> customData = null, string model = null, string languageCustomizationId = null, string acousticCustomizationId = null, string baseModelVersion = null, double? customizationWeight = null, long? inactivityTimeout = null, List<string> keywords = null, float? keywordsThreshold = null, long? maxAlternatives = null, float? wordAlternativesThreshold = null, bool? wordConfidence = null, bool? timestamps = null, bool? profanityFilter = null, bool? smartFormatting = null, bool? speakerLabels = null, string customizationId = null, string grammarName = null, bool? redaction = null, string contentType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Recognize`");
            if (audio == null)
                throw new ArgumentNullException("`audio` is required for `Recognize`");

            RequestObject<SpeechRecognitionResults> req = new RequestObject<SpeechRecognitionResults>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "Recognize"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(model))
            {
                req.Parameters["model"] = model;
            }
            if (!string.IsNullOrEmpty(languageCustomizationId))
            {
                req.Parameters["language_customization_id"] = languageCustomizationId;
            }
            if (!string.IsNullOrEmpty(acousticCustomizationId))
            {
                req.Parameters["acoustic_customization_id"] = acousticCustomizationId;
            }
            if (!string.IsNullOrEmpty(baseModelVersion))
            {
                req.Parameters["base_model_version"] = baseModelVersion;
            }
            if (customizationWeight != null)
            {
                req.Parameters["customization_weight"] = customizationWeight;
            }
            if (inactivityTimeout != null)
            {
                req.Parameters["inactivity_timeout"] = inactivityTimeout;
            }
            if (keywords != null && keywords.Count > 0)
            {
                req.Parameters["keywords"] = string.Join(",", keywords.ToArray());
            }
            if (keywordsThreshold != null)
            {
                req.Parameters["keywords_threshold"] = keywordsThreshold;
            }
            if (maxAlternatives != null)
            {
                req.Parameters["max_alternatives"] = maxAlternatives;
            }
            if (wordAlternativesThreshold != null)
            {
                req.Parameters["word_alternatives_threshold"] = wordAlternativesThreshold;
            }
            if (wordConfidence != null)
            {
                req.Parameters["word_confidence"] = (bool)wordConfidence ? "true" : "false";
            }
            if (timestamps != null)
            {
                req.Parameters["timestamps"] = (bool)timestamps ? "true" : "false";
            }
            if (profanityFilter != null)
            {
                req.Parameters["profanity_filter"] = (bool)profanityFilter ? "true" : "false";
            }
            if (smartFormatting != null)
            {
                req.Parameters["smart_formatting"] = (bool)smartFormatting ? "true" : "false";
            }
            if (speakerLabels != null)
            {
                req.Parameters["speaker_labels"] = (bool)speakerLabels ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }
            if (!string.IsNullOrEmpty(grammarName))
            {
                req.Parameters["grammar_name"] = grammarName;
            }
            if (redaction != null)
            {
                req.Parameters["redaction"] = (bool)redaction ? "true" : "false";
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }
            req.Send = audio;

            req.OnResponse = OnRecognizeResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/recognize");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnRecognizeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<SpeechRecognitionResults> response = new DetailedResponse<SpeechRecognitionResults>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<SpeechRecognitionResults>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnRecognizeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<SpeechRecognitionResults>)req).Callback != null)
                ((RequestObject<SpeechRecognitionResults>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Check a job.
        ///
        /// Returns information about the specified job. The response always includes the status of the job and its
        /// creation and update times. If the status is `completed`, the response includes the results of the
        /// recognition request. You must use credentials for the instance of the service that owns a job to list
        /// information about it.
        ///
        /// You can use the method to retrieve the results of any job, regardless of whether it was submitted with a
        /// callback URL and the `recognitions.completed_with_results` event, and you can retrieve the results multiple
        /// times for as long as they remain available. Use the **Check jobs** method to request information about the
        /// most recent jobs associated with the calling credentials.
        ///
        /// **See also:** [Checking the status and retrieving the results of a
        /// job](https://cloud.ibm.com/docs/services/speech-to-text/async.html#job).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="id">The identifier of the asynchronous job that is to be used for the request. You must make
        /// the request with credentials for the instance of the service that owns the job.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="RecognitionJob" />RecognitionJob</returns>
        public bool CheckJob(Callback<RecognitionJob> callback, string id, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CheckJob`");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("`id` is required for `CheckJob`");

            RequestObject<RecognitionJob> req = new RequestObject<RecognitionJob>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "CheckJob"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnCheckJobResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/recognitions/{0}", id));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCheckJobResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<RecognitionJob> response = new DetailedResponse<RecognitionJob>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<RecognitionJob>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnCheckJobResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<RecognitionJob>)req).Callback != null)
                ((RequestObject<RecognitionJob>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Check jobs.
        ///
        /// Returns the ID and status of the latest 100 outstanding jobs associated with the credentials with which it
        /// is called. The method also returns the creation and update times of each job, and, if a job was created with
        /// a callback URL and a user token, the user token for the job. To obtain the results for a job whose status is
        /// `completed` or not one of the latest 100 outstanding jobs, use the **Check a job** method. A job and its
        /// results remain available until you delete them with the **Delete a job** method or until the job's time to
        /// live expires, whichever comes first.
        ///
        /// **See also:** [Checking the status of the latest
        /// jobs](https://cloud.ibm.com/docs/services/speech-to-text/async.html#jobs).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="RecognitionJobs" />RecognitionJobs</returns>
        public bool CheckJobs(Callback<RecognitionJobs> callback, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CheckJobs`");

            RequestObject<RecognitionJobs> req = new RequestObject<RecognitionJobs>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "CheckJobs"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnCheckJobsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/recognitions");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCheckJobsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<RecognitionJobs> response = new DetailedResponse<RecognitionJobs>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<RecognitionJobs>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnCheckJobsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<RecognitionJobs>)req).Callback != null)
                ((RequestObject<RecognitionJobs>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a job.
        ///
        /// Creates a job for a new asynchronous recognition request. The job is owned by the instance of the service
        /// whose credentials are used to create it. How you learn the status and results of a job depends on the
        /// parameters you include with the job creation request:
        /// * By callback notification: Include the `callback_url` parameter to specify a URL to which the service is to
        /// send callback notifications when the status of the job changes. Optionally, you can also include the
        /// `events` and `user_token` parameters to subscribe to specific events and to specify a string that is to be
        /// included with each notification for the job.
        /// * By polling the service: Omit the `callback_url`, `events`, and `user_token` parameters. You must then use
        /// the **Check jobs** or **Check a job** methods to check the status of the job, using the latter to retrieve
        /// the results when the job is complete.
        ///
        /// The two approaches are not mutually exclusive. You can poll the service for job status or obtain results
        /// from the service manually even if you include a callback URL. In both cases, you can include the
        /// `results_ttl` parameter to specify how long the results are to remain available after the job is complete.
        /// Using the HTTPS **Check a job** method to retrieve results is more secure than receiving them via callback
        /// notification over HTTP because it provides confidentiality in addition to authentication and data integrity.
        ///
        ///
        /// The method supports the same basic parameters as other HTTP and WebSocket recognition requests. It also
        /// supports the following parameters specific to the asynchronous interface:
        /// * `callback_url`
        /// * `events`
        /// * `user_token`
        /// * `results_ttl`
        ///
        /// You can pass a maximum of 1 GB and a minimum of 100 bytes of audio with a request. The service automatically
        /// detects the endianness of the incoming audio and, for audio that includes multiple channels, downmixes the
        /// audio to one-channel mono during transcoding. The method returns only final results; to enable interim
        /// results, use the WebSocket API.
        ///
        /// **See also:** [Creating a job](https://cloud.ibm.com/docs/services/speech-to-text/async.html#create).
        ///
        /// ### Streaming mode
        ///
        ///  For requests to transcribe live audio as it becomes available, you must set the `Transfer-Encoding` header
        /// to `chunked` to use streaming mode. In streaming mode, the service closes the connection (status code 408)
        /// if it does not receive at least 15 seconds of audio (including silence) in any 30-second period. The service
        /// also closes the connection (status code 400) if it detects no speech for `inactivity_timeout` seconds of
        /// streaming audio; use the `inactivity_timeout` parameter to change the default of 30 seconds.
        ///
        /// **See also:**
        /// * [Audio transmission](https://cloud.ibm.com/docs/services/speech-to-text/input.html#transmission)
        /// * [Timeouts](https://cloud.ibm.com/docs/services/speech-to-text/input.html#timeouts)
        ///
        /// ### Audio formats (content types)
        ///
        ///  The service accepts audio in the following formats (MIME types).
        /// * For formats that are labeled **Required**, you must use the `Content-Type` header with the request to
        /// specify the format of the audio.
        /// * For all other formats, you can omit the `Content-Type` header or specify `application/octet-stream` with
        /// the header to have the service automatically detect the format of the audio. (With the `curl` command, you
        /// can specify either `"Content-Type:"` or `"Content-Type: application/octet-stream"`.)
        ///
        /// Where indicated, the format that you specify must include the sampling rate and can optionally include the
        /// number of channels and the endianness of the audio.
        /// * `audio/alaw` (**Required.** Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/basic` (**Required.** Use only with narrowband models.)
        /// * `audio/flac`
        /// * `audio/g729` (Use only with narrowband models.)
        /// * `audio/l16` (**Required.** Specify the sampling rate (`rate`) and optionally the number of channels
        /// (`channels`) and endianness (`endianness`) of the audio.)
        /// * `audio/mp3`
        /// * `audio/mpeg`
        /// * `audio/mulaw` (**Required.** Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/ogg` (The service automatically detects the codec of the input audio.)
        /// * `audio/ogg;codecs=opus`
        /// * `audio/ogg;codecs=vorbis`
        /// * `audio/wav` (Provide audio with a maximum of nine channels.)
        /// * `audio/webm` (The service automatically detects the codec of the input audio.)
        /// * `audio/webm;codecs=opus`
        /// * `audio/webm;codecs=vorbis`
        ///
        /// The sampling rate of the audio must match the sampling rate of the model for the recognition request: for
        /// broadband models, at least 16 kHz; for narrowband models, at least 8 kHz. If the sampling rate of the audio
        /// is higher than the minimum required rate, the service down-samples the audio to the appropriate rate. If the
        /// sampling rate of the audio is lower than the minimum required rate, the request fails.
        ///
        ///  **See also:** [Audio formats](https://cloud.ibm.com/docs/services/speech-to-text/audio-formats.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="audio">The audio to transcribe.</param>
        /// <param name="model">The identifier of the model that is to be used for the recognition request. See
        /// [Languages and models](https://cloud.ibm.com/docs/services/speech-to-text/models.html). (optional, default
        /// to en-US_BroadbandModel)</param>
        /// <param name="callbackUrl">A URL to which callback notifications are to be sent. The URL must already be
        /// successfully white-listed by using the **Register a callback** method. You can include the same callback URL
        /// with any number of job creation requests. Omit the parameter to poll the service for job completion and
        /// results.
        ///
        /// Use the `user_token` parameter to specify a unique user-specified string with each job to differentiate the
        /// callback notifications for the jobs. (optional)</param>
        /// <param name="events">If the job includes a callback URL, a comma-separated list of notification events to
        /// which to subscribe. Valid events are
        /// * `recognitions.started` generates a callback notification when the service begins to process the job.
        /// * `recognitions.completed` generates a callback notification when the job is complete. You must use the
        /// **Check a job** method to retrieve the results before they time out or are deleted.
        /// * `recognitions.completed_with_results` generates a callback notification when the job is complete. The
        /// notification includes the results of the request.
        /// * `recognitions.failed` generates a callback notification if the service experiences an error while
        /// processing the job.
        ///
        /// The `recognitions.completed` and `recognitions.completed_with_results` events are incompatible. You can
        /// specify only of the two events.
        ///
        /// If the job includes a callback URL, omit the parameter to subscribe to the default events:
        /// `recognitions.started`, `recognitions.completed`, and `recognitions.failed`. If the job does not include a
        /// callback URL, omit the parameter. (optional)</param>
        /// <param name="userToken">If the job includes a callback URL, a user-specified string that the service is to
        /// include with each callback notification for the job; the token allows the user to maintain an internal
        /// mapping between jobs and notification events. If the job does not include a callback URL, omit the
        /// parameter. (optional)</param>
        /// <param name="resultsTtl">The number of minutes for which the results are to be available after the job has
        /// finished. If not delivered via a callback, the results must be retrieved within this time. Omit the
        /// parameter to use a time to live of one week. The parameter is valid with or without a callback URL.
        /// (optional)</param>
        /// <param name="languageCustomizationId">The customization ID (GUID) of a custom language model that is to be
        /// used with the recognition request. The base model of the specified custom language model must match the
        /// model specified with the `model` parameter. You must make the request with credentials for the instance of
        /// the service that owns the custom model. By default, no custom language model is used. See [Custom
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input).
        ///
        /// **Note:** Use this parameter instead of the deprecated `customization_id` parameter. (optional)</param>
        /// <param name="acousticCustomizationId">The customization ID (GUID) of a custom acoustic model that is to be
        /// used with the recognition request. The base model of the specified custom acoustic model must match the
        /// model specified with the `model` parameter. You must make the request with credentials for the instance of
        /// the service that owns the custom model. By default, no custom acoustic model is used. See [Custom
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input). (optional)</param>
        /// <param name="baseModelVersion">The version of the specified base model that is to be used with recognition
        /// request. Multiple versions of a base model can exist when a model is updated for internal improvements. The
        /// parameter is intended primarily for use with custom models that have been upgraded for a new base model. The
        /// default value depends on whether the parameter is used with or without a custom model. See [Base model
        /// version](https://cloud.ibm.com/docs/services/speech-to-text/input.html#version). (optional)</param>
        /// <param name="customizationWeight">If you specify the customization ID (GUID) of a custom language model with
        /// the recognition request, the customization weight tells the service how much weight to give to words from
        /// the custom language model compared to those from the base model for the current request.
        ///
        /// Specify a value between 0.0 and 1.0. Unless a different customization weight was specified for the custom
        /// model when it was trained, the default value is 0.3. A customization weight that you specify overrides a
        /// weight that was specified when the custom model was trained.
        ///
        /// The default value yields the best performance in general. Assign a higher value if your audio makes frequent
        /// use of OOV words from the custom model. Use caution when setting the weight: a higher value can improve the
        /// accuracy of phrases from the custom model's domain, but it can negatively affect performance on non-domain
        /// phrases.
        ///
        /// See [Custom models](https://cloud.ibm.com/docs/services/speech-to-text/input.html#custom-input).
        /// (optional)</param>
        /// <param name="inactivityTimeout">The time in seconds after which, if only silence (no speech) is detected in
        /// streaming audio, the connection is closed with a 400 error. The parameter is useful for stopping audio
        /// submission from a live microphone when a user simply walks away. Use `-1` for infinity. See [Inactivity
        /// timeout](https://cloud.ibm.com/docs/services/speech-to-text/input.html#timeouts-inactivity). (optional,
        /// default to 30)</param>
        /// <param name="keywords">An array of keyword strings to spot in the audio. Each keyword string can include one
        /// or more string tokens. Keywords are spotted only in the final results, not in interim hypotheses. If you
        /// specify any keywords, you must also specify a keywords threshold. You can spot a maximum of 1000 keywords.
        /// Omit the parameter or specify an empty array if you do not need to spot keywords. See [Keyword
        /// spotting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#keyword_spotting).
        /// (optional)</param>
        /// <param name="keywordsThreshold">A confidence value that is the lower bound for spotting a keyword. A word is
        /// considered to match a keyword if its confidence is greater than or equal to the threshold. Specify a
        /// probability between 0.0 and 1.0. If you specify a threshold, you must also specify one or more keywords. The
        /// service performs no keyword spotting if you omit either parameter. See [Keyword
        /// spotting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#keyword_spotting).
        /// (optional)</param>
        /// <param name="maxAlternatives">The maximum number of alternative transcripts that the service is to return.
        /// By default, the service returns a single transcript. If you specify a value of `0`, the service uses the
        /// default value, `1`. See [Maximum
        /// alternatives](https://cloud.ibm.com/docs/services/speech-to-text/output.html#max_alternatives). (optional,
        /// default to 1)</param>
        /// <param name="wordAlternativesThreshold">A confidence value that is the lower bound for identifying a
        /// hypothesis as a possible word alternative (also known as "Confusion Networks"). An alternative word is
        /// considered if its confidence is greater than or equal to the threshold. Specify a probability between 0.0
        /// and 1.0. By default, the service computes no alternative words. See [Word
        /// alternatives](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_alternatives).
        /// (optional)</param>
        /// <param name="wordConfidence">If `true`, the service returns a confidence measure in the range of 0.0 to 1.0
        /// for each word. By default, the service returns no word confidence scores. See [Word
        /// confidence](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_confidence). (optional,
        /// default to false)</param>
        /// <param name="timestamps">If `true`, the service returns time alignment for each word. By default, no
        /// timestamps are returned. See [Word
        /// timestamps](https://cloud.ibm.com/docs/services/speech-to-text/output.html#word_timestamps). (optional,
        /// default to false)</param>
        /// <param name="profanityFilter">If `true`, the service filters profanity from all output except for keyword
        /// results by replacing inappropriate words with a series of asterisks. Set the parameter to `false` to return
        /// results with no censoring. Applies to US English transcription only. See [Profanity
        /// filtering](https://cloud.ibm.com/docs/services/speech-to-text/output.html#profanity_filter). (optional,
        /// default to true)</param>
        /// <param name="smartFormatting">If `true`, the service converts dates, times, series of digits and numbers,
        /// phone numbers, currency values, and internet addresses into more readable, conventional representations in
        /// the final transcript of a recognition request. For US English, the service also converts certain keyword
        /// strings to punctuation symbols. By default, the service performs no smart formatting.
        ///
        /// **Note:** Applies to US English, Japanese, and Spanish transcription only.
        ///
        /// See [Smart formatting](https://cloud.ibm.com/docs/services/speech-to-text/output.html#smart_formatting).
        /// (optional, default to false)</param>
        /// <param name="speakerLabels">If `true`, the response includes labels that identify which words were spoken by
        /// which participants in a multi-person exchange. By default, the service returns no speaker labels. Setting
        /// `speaker_labels` to `true` forces the `timestamps` parameter to be `true`, regardless of whether you specify
        /// `false` for the parameter.
        ///
        /// **Note:** Applies to US English, Japanese, and Spanish transcription only. To determine whether a language
        /// model supports speaker labels, you can also use the **Get a model** method and check that the attribute
        /// `speaker_labels` is set to `true`.
        ///
        /// See [Speaker labels](https://cloud.ibm.com/docs/services/speech-to-text/output.html#speaker_labels).
        /// (optional, default to false)</param>
        /// <param name="customizationId">**Deprecated.** Use the `language_customization_id` parameter to specify the
        /// customization ID (GUID) of a custom language model that is to be used with the recognition request. Do not
        /// specify both parameters with a request. (optional)</param>
        /// <param name="grammarName">The name of a grammar that is to be used with the recognition request. If you
        /// specify a grammar, you must also use the `language_customization_id` parameter to specify the name of the
        /// custom language model for which the grammar is defined. The service recognizes only strings that are
        /// recognized by the specified grammar; it does not recognize other custom words from the model's words
        /// resource. See [Grammars](https://cloud.ibm.com/docs/services/speech-to-text/input.html#grammars-input).
        /// (optional)</param>
        /// <param name="redaction">If `true`, the service redacts, or masks, numeric data from final transcripts. The
        /// feature redacts any number that has three or more consecutive digits by replacing each digit with an `X`
        /// character. It is intended to redact sensitive numeric data, such as credit card numbers. By default, the
        /// service performs no redaction.
        ///
        /// When you enable redaction, the service automatically enables smart formatting, regardless of whether you
        /// explicitly disable that feature. To ensure maximum security, the service also disables keyword spotting
        /// (ignores the `keywords` and `keywords_threshold` parameters) and returns only a single final transcript
        /// (forces the `max_alternatives` parameter to be `1`).
        ///
        /// **Note:** Applies to US English, Japanese, and Korean transcription only.
        ///
        /// See [Numeric redaction](https://cloud.ibm.com/docs/services/speech-to-text/output.html#redaction).
        /// (optional, default to false)</param>
        /// <param name="contentType">The format (MIME type) of the audio. For more information about specifying an
        /// audio format, see **Audio formats (content types)** in the method description. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="RecognitionJob" />RecognitionJob</returns>
        public bool CreateJob(Callback<RecognitionJob> callback, byte[] audio, Dictionary<string, object> customData = null, string model = null, string callbackUrl = null, string events = null, string userToken = null, long? resultsTtl = null, string languageCustomizationId = null, string acousticCustomizationId = null, string baseModelVersion = null, double? customizationWeight = null, long? inactivityTimeout = null, List<string> keywords = null, float? keywordsThreshold = null, long? maxAlternatives = null, float? wordAlternativesThreshold = null, bool? wordConfidence = null, bool? timestamps = null, bool? profanityFilter = null, bool? smartFormatting = null, bool? speakerLabels = null, string customizationId = null, string grammarName = null, bool? redaction = null, string contentType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateJob`");
            if (audio == null)
                throw new ArgumentNullException("`audio` is required for `CreateJob`");

            RequestObject<RecognitionJob> req = new RequestObject<RecognitionJob>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "CreateJob"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(model))
            {
                req.Parameters["model"] = model;
            }
            if (!string.IsNullOrEmpty(callbackUrl))
            {
                req.Parameters["callback_url"] = callbackUrl;
            }
            if (!string.IsNullOrEmpty(events))
            {
                req.Parameters["events"] = events;
            }
            if (!string.IsNullOrEmpty(userToken))
            {
                req.Parameters["user_token"] = userToken;
            }
            if (resultsTtl != null)
            {
                req.Parameters["results_ttl"] = resultsTtl;
            }
            if (!string.IsNullOrEmpty(languageCustomizationId))
            {
                req.Parameters["language_customization_id"] = languageCustomizationId;
            }
            if (!string.IsNullOrEmpty(acousticCustomizationId))
            {
                req.Parameters["acoustic_customization_id"] = acousticCustomizationId;
            }
            if (!string.IsNullOrEmpty(baseModelVersion))
            {
                req.Parameters["base_model_version"] = baseModelVersion;
            }
            if (customizationWeight != null)
            {
                req.Parameters["customization_weight"] = customizationWeight;
            }
            if (inactivityTimeout != null)
            {
                req.Parameters["inactivity_timeout"] = inactivityTimeout;
            }
            if (keywords != null && keywords.Count > 0)
            {
                req.Parameters["keywords"] = string.Join(",", keywords.ToArray());
            }
            if (keywordsThreshold != null)
            {
                req.Parameters["keywords_threshold"] = keywordsThreshold;
            }
            if (maxAlternatives != null)
            {
                req.Parameters["max_alternatives"] = maxAlternatives;
            }
            if (wordAlternativesThreshold != null)
            {
                req.Parameters["word_alternatives_threshold"] = wordAlternativesThreshold;
            }
            if (wordConfidence != null)
            {
                req.Parameters["word_confidence"] = (bool)wordConfidence ? "true" : "false";
            }
            if (timestamps != null)
            {
                req.Parameters["timestamps"] = (bool)timestamps ? "true" : "false";
            }
            if (profanityFilter != null)
            {
                req.Parameters["profanity_filter"] = (bool)profanityFilter ? "true" : "false";
            }
            if (smartFormatting != null)
            {
                req.Parameters["smart_formatting"] = (bool)smartFormatting ? "true" : "false";
            }
            if (speakerLabels != null)
            {
                req.Parameters["speaker_labels"] = (bool)speakerLabels ? "true" : "false";
            }
            if (!string.IsNullOrEmpty(customizationId))
            {
                req.Parameters["customization_id"] = customizationId;
            }
            if (!string.IsNullOrEmpty(grammarName))
            {
                req.Parameters["grammar_name"] = grammarName;
            }
            if (redaction != null)
            {
                req.Parameters["redaction"] = (bool)redaction ? "true" : "false";
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }
            req.Send = audio;

            req.OnResponse = OnCreateJobResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/recognitions");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateJobResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<RecognitionJob> response = new DetailedResponse<RecognitionJob>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<RecognitionJob>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnCreateJobResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<RecognitionJob>)req).Callback != null)
                ((RequestObject<RecognitionJob>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a job.
        ///
        /// Deletes the specified job. You cannot delete a job that the service is actively processing. Once you delete
        /// a job, its results are no longer available. The service automatically deletes a job and its results when the
        /// time to live for the results expires. You must use credentials for the instance of the service that owns a
        /// job to delete it.
        ///
        /// **See also:** [Deleting a job](https://cloud.ibm.com/docs/services/speech-to-text/async.html#delete-async).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="id">The identifier of the asynchronous job that is to be used for the request. You must make
        /// the request with credentials for the instance of the service that owns the job.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteJob(Callback<object> callback, string id, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteJob`");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("`id` is required for `DeleteJob`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteJob"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteJobResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/recognitions/{0}", id));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteJobResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteJobResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Register a callback.
        ///
        /// Registers a callback URL with the service for use with subsequent asynchronous recognition requests. The
        /// service attempts to register, or white-list, the callback URL if it is not already registered by sending a
        /// `GET` request to the callback URL. The service passes a random alphanumeric challenge string via the
        /// `challenge_string` parameter of the request. The request includes an `Accept` header that specifies
        /// `text/plain` as the required response type.
        ///
        /// To be registered successfully, the callback URL must respond to the `GET` request from the service. The
        /// response must send status code 200 and must include the challenge string in its body. Set the `Content-Type`
        /// response header to `text/plain`. Upon receiving this response, the service responds to the original
        /// registration request with response code 201.
        ///
        /// The service sends only a single `GET` request to the callback URL. If the service does not receive a reply
        /// with a response code of 200 and a body that echoes the challenge string sent by the service within five
        /// seconds, it does not white-list the URL; it instead sends status code 400 in response to the **Register a
        /// callback** request. If the requested callback URL is already white-listed, the service responds to the
        /// initial registration request with response code 200.
        ///
        /// If you specify a user secret with the request, the service uses it as a key to calculate an HMAC-SHA1
        /// signature of the challenge string in its response to the `POST` request. It sends this signature in the
        /// `X-Callback-Signature` header of its `GET` request to the URL during registration. It also uses the secret
        /// to calculate a signature over the payload of every callback notification that uses the URL. The signature
        /// provides authentication and data integrity for HTTP communications.
        ///
        /// After you successfully register a callback URL, you can use it with an indefinite number of recognition
        /// requests. You can register a maximum of 20 callback URLS in a one-hour span of time.
        ///
        /// **See also:** [Registering a callback
        /// URL](https://cloud.ibm.com/docs/services/speech-to-text/async.html#register).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="callbackUrl">An HTTP or HTTPS URL to which callback notifications are to be sent. To be
        /// white-listed, the URL must successfully echo the challenge string during URL verification. During
        /// verification, the client can also check the signature that the service sends in the `X-Callback-Signature`
        /// header to verify the origin of the request.</param>
        /// <param name="userSecret">A user-specified string that the service uses to generate the HMAC-SHA1 signature
        /// that it sends via the `X-Callback-Signature` header. The service includes the header during URL verification
        /// and with every notification sent to the callback URL. It calculates the signature over the payload of the
        /// notification. If you omit the parameter, the service does not send the header. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="RegisterStatus" />RegisterStatus</returns>
        public bool RegisterCallback(Callback<RegisterStatus> callback, string callbackUrl, Dictionary<string, object> customData = null, string userSecret = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `RegisterCallback`");
            if (string.IsNullOrEmpty(callbackUrl))
                throw new ArgumentNullException("`callbackUrl` is required for `RegisterCallback`");

            RequestObject<RegisterStatus> req = new RequestObject<RegisterStatus>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "RegisterCallback"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                req.Parameters["callback_url"] = callbackUrl;
            }
            if (!string.IsNullOrEmpty(userSecret))
            {
                req.Parameters["user_secret"] = userSecret;
            }

            req.OnResponse = OnRegisterCallbackResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/register_callback");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnRegisterCallbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<RegisterStatus> response = new DetailedResponse<RegisterStatus>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<RegisterStatus>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnRegisterCallbackResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<RegisterStatus>)req).Callback != null)
                ((RequestObject<RegisterStatus>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Unregister a callback.
        ///
        /// Unregisters a callback URL that was previously white-listed with a **Register a callback** request for use
        /// with the asynchronous interface. Once unregistered, the URL can no longer be used with asynchronous
        /// recognition requests.
        ///
        /// **See also:** [Unregistering a callback
        /// URL](https://cloud.ibm.com/docs/services/speech-to-text/async.html#unregister).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="callbackUrl">The callback URL that is to be unregistered.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool UnregisterCallback(Callback<object> callback, string callbackUrl, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UnregisterCallback`");
            if (string.IsNullOrEmpty(callbackUrl))
                throw new ArgumentNullException("`callbackUrl` is required for `UnregisterCallback`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "UnregisterCallback"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                req.Parameters["callback_url"] = callbackUrl;
            }

            req.OnResponse = OnUnregisterCallbackResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/unregister_callback");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUnregisterCallbackResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnUnregisterCallbackResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a custom language model.
        ///
        /// Creates a new custom language model for a specified base model. The custom language model can be used only
        /// with the base model for which it is created. The model is owned by the instance of the service whose
        /// credentials are used to create it.
        ///
        /// **See also:** [Create a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-create.html#createModel-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">A user-defined name for the new custom language model. Use a name that is unique among
        /// all custom language models that you own. Use a localized name that matches the language of the custom model.
        /// Use a name that describes the domain of the custom model, such as `Medical custom model` or `Legal custom
        /// model`.</param>
        /// <param name="baseModelName">The name of the base language model that is to be customized by the new custom
        /// language model. The new custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports language model customization, use the **Get a model** method and
        /// check that the attribute `custom_language_model` is set to `true`. You can also refer to [Language support
        /// for customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).</param>
        /// <param name="dialect">The dialect of the specified language that is to be used with the custom language
        /// model. The parameter is meaningful only for Spanish models, for which the service creates a custom language
        /// model that is suited for speech in one of the following dialects:
        /// * `es-ES` for Castilian Spanish (the default)
        /// * `es-LA` for Latin American Spanish
        /// * `es-US` for North American (Mexican) Spanish
        ///
        /// A specified dialect must be valid for the base model. By default, the dialect matches the language of the
        /// base model; for example, `en-US` for either of the US English language models. (optional)</param>
        /// <param name="description">A description of the new custom language model. Use a localized description that
        /// matches the language of the custom model. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="LanguageModel" />LanguageModel</returns>
        public bool CreateLanguageModel(Callback<LanguageModel> callback, string name, string baseModelName, Dictionary<string, object> customData = null, string dialect = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateLanguageModel`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateLanguageModel`");
            if (string.IsNullOrEmpty(baseModelName))
                throw new ArgumentNullException("`baseModelName` is required for `CreateLanguageModel`");

            RequestObject<LanguageModel> req = new RequestObject<LanguageModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "CreateLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(baseModelName))
                bodyObject["base_model_name"] = baseModelName;
            if (!string.IsNullOrEmpty(dialect))
                bodyObject["dialect"] = dialect;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LanguageModel> response = new DetailedResponse<LanguageModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LanguageModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnCreateLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LanguageModel>)req).Callback != null)
                ((RequestObject<LanguageModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom language model.
        ///
        /// Deletes an existing custom language model. The custom model cannot be deleted if another request, such as
        /// adding a corpus or grammar to the model, is currently being processed. You must use credentials for the
        /// instance of the service that owns a model to delete it.
        ///
        /// **See also:** [Deleting a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-models.html#deleteModel-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteLanguageModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteLanguageModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteLanguageModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a custom language model.
        ///
        /// Gets information about a specified custom language model. You must use credentials for the instance of the
        /// service that owns a model to list information about it.
        ///
        /// **See also:** [Listing custom language
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/language-models.html#listModels-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="LanguageModel" />LanguageModel</returns>
        public bool GetLanguageModel(Callback<LanguageModel> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetLanguageModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetLanguageModel`");

            RequestObject<LanguageModel> req = new RequestObject<LanguageModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LanguageModel> response = new DetailedResponse<LanguageModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LanguageModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LanguageModel>)req).Callback != null)
                ((RequestObject<LanguageModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List custom language models.
        ///
        /// Lists information about all custom language models that are owned by an instance of the service. Use the
        /// `language` parameter to see all custom language models for the specified language. Omit the parameter to see
        /// all custom language models for all languages. You must use credentials for the instance of the service that
        /// owns a model to list information about it.
        ///
        /// **See also:** [Listing custom language
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/language-models.html#listModels-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The identifier of the language for which custom language or custom acoustic models
        /// are to be returned (for example, `en-US`). Omit the parameter to see all custom language or custom acoustic
        /// models that are owned by the requesting credentials. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="LanguageModels" />LanguageModels</returns>
        public bool ListLanguageModels(Callback<LanguageModels> callback, Dictionary<string, object> customData = null, string language = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListLanguageModels`");

            RequestObject<LanguageModels> req = new RequestObject<LanguageModels>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListLanguageModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(language))
            {
                req.Parameters["language"] = language;
            }

            req.OnResponse = OnListLanguageModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListLanguageModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<LanguageModels> response = new DetailedResponse<LanguageModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<LanguageModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListLanguageModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<LanguageModels>)req).Callback != null)
                ((RequestObject<LanguageModels>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Reset a custom language model.
        ///
        /// Resets a custom language model by removing all corpora, grammars, and words from the model. Resetting a
        /// custom language model initializes the model to its state when it was first created. Metadata such as the
        /// name and language of the model are preserved, but the model's words resource is removed and must be
        /// re-created. You must use credentials for the instance of the service that owns a model to reset it.
        ///
        /// **See also:** [Resetting a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-models.html#resetModel-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool ResetLanguageModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ResetLanguageModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ResetLanguageModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ResetLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnResetLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/reset", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnResetLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnResetLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Train a custom language model.
        ///
        /// Initiates the training of a custom language model with new resources such as corpora, grammars, and custom
        /// words. After adding, modifying, or deleting resources for a custom language model, use this method to begin
        /// the actual training of the model on the latest data. You can specify whether the custom language model is to
        /// be trained with all words from its words resource or only with words that were added or modified by the user
        /// directly. You must use credentials for the instance of the service that owns a model to train it.
        ///
        /// The training method is asynchronous. It can take on the order of minutes to complete depending on the amount
        /// of data on which the service is being trained and the current load on the service. The method returns an
        /// HTTP 200 response code to indicate that the training process has begun.
        ///
        /// You can monitor the status of the training by using the **Get a custom language model** method to poll the
        /// model's status. Use a loop to check the status every 10 seconds. The method returns a `LanguageModel` object
        /// that includes `status` and `progress` fields. A status of `available` means that the custom model is trained
        /// and ready to use. The service cannot accept subsequent training requests or requests to add new resources
        /// until the existing request completes.
        ///
        /// Training can fail to start for the following reasons:
        /// * The service is currently handling another request for the custom model, such as another training request
        /// or a request to add a corpus or grammar to the model.
        /// * No training data have been added to the custom model.
        /// * One or more words that were added to the custom model have invalid sounds-like pronunciations that you
        /// must fix.
        ///
        /// **See also:** [Train the custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-create.html#trainModel-language).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="wordTypeToAdd">The type of words from the custom language model's words resource on which to
        /// train the model:
        /// * `all` (the default) trains the model on all new words, regardless of whether they were extracted from
        /// corpora or grammars or were added or modified by the user.
        /// * `user` trains the model only on new words that were added or modified by the user directly. The model is
        /// not trained on new words extracted from corpora or grammars. (optional, default to all)</param>
        /// <param name="customizationWeight">Specifies a customization weight for the custom language model. The
        /// customization weight tells the service how much weight to give to words from the custom language model
        /// compared to those from the base model for speech recognition. Specify a value between 0.0 and 1.0; the
        /// default is 0.3.
        ///
        /// The default value yields the best performance in general. Assign a higher value if your audio makes frequent
        /// use of OOV words from the custom model. Use caution when setting the weight: a higher value can improve the
        /// accuracy of phrases from the custom model's domain, but it can negatively affect performance on non-domain
        /// phrases.
        ///
        /// The value that you assign is used for all recognition requests that use the model. You can override it for
        /// any recognition request by specifying a customization weight for that request. (optional, default to
        /// 0.3)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool TrainLanguageModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null, string wordTypeToAdd = null, double? customizationWeight = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `TrainLanguageModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `TrainLanguageModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "TrainLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(wordTypeToAdd))
            {
                req.Parameters["word_type_to_add"] = wordTypeToAdd;
            }
            if (customizationWeight != null)
            {
                req.Parameters["customization_weight"] = customizationWeight;
            }

            req.OnResponse = OnTrainLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/train", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnTrainLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnTrainLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Upgrade a custom language model.
        ///
        /// Initiates the upgrade of a custom language model to the latest version of its base language model. The
        /// upgrade method is asynchronous. It can take on the order of minutes to complete depending on the amount of
        /// data in the custom model and the current load on the service. A custom model must be in the `ready` or
        /// `available` state to be upgraded. You must use credentials for the instance of the service that owns a model
        /// to upgrade it.
        ///
        /// The method returns an HTTP 200 response code to indicate that the upgrade process has begun successfully.
        /// You can monitor the status of the upgrade by using the **Get a custom language model** method to poll the
        /// model's status. The method returns a `LanguageModel` object that includes `status` and `progress` fields.
        /// Use a loop to check the status every 10 seconds. While it is being upgraded, the custom model has the status
        /// `upgrading`. When the upgrade is complete, the model resumes the status that it had prior to upgrade. The
        /// service cannot accept subsequent requests for the model until the upgrade completes.
        ///
        /// **See also:** [Upgrading a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/custom-upgrade.html#upgradeLanguage).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool UpgradeLanguageModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpgradeLanguageModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `UpgradeLanguageModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "UpgradeLanguageModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnUpgradeLanguageModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/upgrade_model", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpgradeLanguageModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnUpgradeLanguageModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a corpus.
        ///
        /// Adds a single corpus text file of new training data to a custom language model. Use multiple requests to
        /// submit multiple corpus text files. You must use credentials for the instance of the service that owns a
        /// model to add a corpus to it. Adding a corpus does not affect the custom language model until you train the
        /// model for the new data by using the **Train a custom language model** method.
        ///
        /// Submit a plain text file that contains sample sentences from the domain of interest to enable the service to
        /// extract words in context. The more sentences you add that represent the context in which speakers use words
        /// from the domain, the better the service's recognition accuracy.
        ///
        /// The call returns an HTTP 201 response code if the corpus is valid. The service then asynchronously processes
        /// the contents of the corpus and automatically extracts new words that it finds. This can take on the order of
        /// a minute or two to complete depending on the total number of words and the number of new words in the
        /// corpus, as well as the current load on the service. You cannot submit requests to add additional resources
        /// to the custom model or to train the model until the service's analysis of the corpus for the current request
        /// completes. Use the **List a corpus** method to check the status of the analysis.
        ///
        /// The service auto-populates the model's words resource with words from the corpus that are not found in its
        /// base vocabulary. These are referred to as out-of-vocabulary (OOV) words. You can use the **List custom
        /// words** method to examine the words resource. You can use other words method to eliminate typos and modify
        /// how words are pronounced as needed.
        ///
        /// To add a corpus file that has the same name as an existing corpus, set the `allow_overwrite` parameter to
        /// `true`; otherwise, the request fails. Overwriting an existing corpus causes the service to process the
        /// corpus text file and extract OOV words anew. Before doing so, it removes any OOV words associated with the
        /// existing corpus from the model's words resource unless they were also added by another corpus or grammar, or
        /// they have been modified in some way with the **Add custom words** or **Add a custom word** method.
        ///
        /// The service limits the overall amount of data that you can add to a custom model to a maximum of 10 million
        /// total words from all sources combined. Also, you can add no more than 30 thousand custom (OOV) words to a
        /// model. This includes words that the service extracts from corpora and grammars, and words that you add
        /// directly.
        ///
        /// **See also:**
        /// * [Working with
        /// corpora](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#workingCorpora)
        /// * [Add corpora to the custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-create.html#addCorpora).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="corpusName">The name of the new corpus for the custom language model. Use a localized name that
        /// matches the language of the custom model and reflects the contents of the corpus.
        /// * Include a maximum of 128 characters in the name.
        /// * Do not include spaces, slashes, or backslashes in the name.
        /// * Do not use the name of an existing corpus or grammar that is already defined for the custom model.
        /// * Do not use the name `user`, which is reserved by the service to denote custom words that are added or
        /// modified by the user.</param>
        /// <param name="corpusFile">A plain text file that contains the training data for the corpus. Encode the file
        /// in UTF-8 if it contains non-ASCII characters; the service assumes UTF-8 encoding if it encounters non-ASCII
        /// characters.
        ///
        /// Make sure that you know the character encoding of the file. You must use that encoding when working with the
        /// words in the custom language model. For more information, see [Character
        /// encoding](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#charEncoding).
        ///
        /// With the `curl` command, use the `--data-binary` option to upload the file for the request.</param>
        /// <param name="allowOverwrite">If `true`, the specified corpus overwrites an existing corpus with the same
        /// name. If `false`, the request fails if a corpus with the same name already exists. The parameter has no
        /// effect if a corpus with the same name does not already exist. (optional, default to false)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool AddCorpus(Callback<object> callback, string customizationId, string corpusName, System.IO.FileStream corpusFile, Dictionary<string, object> customData = null, bool? allowOverwrite = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddCorpus`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddCorpus`");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("`corpusName` is required for `AddCorpus`");
            if (corpusFile == null)
                throw new ArgumentNullException("`corpusFile` is required for `AddCorpus`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "AddCorpus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (corpusFile != null)
            {
                req.Forms["corpus_file"] = new RESTConnector.Form(corpusFile, corpusFile.Name, "text/plain");
            }
            if (allowOverwrite != null)
            {
                req.Parameters["allow_overwrite"] = (bool)allowOverwrite ? "true" : "false";
            }

            req.OnResponse = OnAddCorpusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/corpora/{1}", customizationId, corpusName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddCorpusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnAddCorpusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a corpus.
        ///
        /// Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV)
        /// words that are associated with the corpus from the custom model's words resource unless they were also added
        /// by another corpus or grammar, or they were modified in some way with the **Add custom words** or **Add a
        /// custom word** method. Removing a corpus does not affect the custom model until you train the model with the
        /// **Train a custom language model** method. You must use credentials for the instance of the service that owns
        /// a model to delete its corpora.
        ///
        /// **See also:** [Deleting a corpus from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-corpora.html#deleteCorpus).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="corpusName">The name of the corpus for the custom language model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteCorpus(Callback<object> callback, string customizationId, string corpusName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCorpus`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteCorpus`");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("`corpusName` is required for `DeleteCorpus`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteCorpus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteCorpusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/corpora/{1}", customizationId, corpusName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteCorpusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteCorpusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a corpus.
        ///
        /// Gets information about a corpus from a custom language model. The information includes the total number of
        /// words and out-of-vocabulary (OOV) words, name, and status of the corpus. You must use credentials for the
        /// instance of the service that owns a model to list its corpora.
        ///
        /// **See also:** [Listing corpora for a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-corpora.html#listCorpora).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="corpusName">The name of the corpus for the custom language model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Corpus" />Corpus</returns>
        public bool GetCorpus(Callback<Corpus> callback, string customizationId, string corpusName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCorpus`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetCorpus`");
            if (string.IsNullOrEmpty(corpusName))
                throw new ArgumentNullException("`corpusName` is required for `GetCorpus`");

            RequestObject<Corpus> req = new RequestObject<Corpus>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetCorpus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetCorpusResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/corpora/{1}", customizationId, corpusName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetCorpusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Corpus> response = new DetailedResponse<Corpus>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Corpus>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetCorpusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Corpus>)req).Callback != null)
                ((RequestObject<Corpus>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List corpora.
        ///
        /// Lists information about all corpora from a custom language model. The information includes the total number
        /// of words and out-of-vocabulary (OOV) words, name, and status of each corpus. You must use credentials for
        /// the instance of the service that owns a model to list its corpora.
        ///
        /// **See also:** [Listing corpora for a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-corpora.html#listCorpora).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Corpora" />Corpora</returns>
        public bool ListCorpora(Callback<Corpora> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCorpora`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListCorpora`");

            RequestObject<Corpora> req = new RequestObject<Corpora>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListCorpora"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListCorporaResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/corpora", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListCorporaResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Corpora> response = new DetailedResponse<Corpora>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Corpora>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListCorporaResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Corpora>)req).Callback != null)
                ((RequestObject<Corpora>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a custom word.
        ///
        /// Adds a custom word to a custom language model. The service populates the words resource for a custom model
        /// with out-of-vocabulary (OOV) words from each corpus or grammar that is added to the model. You can use this
        /// method to add a word or to modify an existing word in the words resource. The words resource for a model can
        /// contain a maximum of 30 thousand custom (OOV) words. This includes words that the service extracts from
        /// corpora and grammars and words that you add directly.
        ///
        /// You must use credentials for the instance of the service that owns a model to add or modify a custom word
        /// for the model. Adding or modifying a custom word does not affect the custom model until you train the model
        /// for the new data by using the **Train a custom language model** method.
        ///
        /// Use the `word_name` parameter to specify the custom word that is to be added or modified. Use the
        /// `CustomWord` object to provide one or both of the optional `sounds_like` and `display_as` fields for the
        /// word.
        /// * The `sounds_like` field provides an array of one or more pronunciations for the word. Use the parameter to
        /// specify how the word can be pronounced by users. Use the parameter for words that are difficult to
        /// pronounce, foreign words, acronyms, and so on. For example, you might specify that the word `IEEE` can sound
        /// like `i triple e`. You can specify a maximum of five sounds-like pronunciations for a word.
        /// * The `display_as` field provides a different way of spelling the word in a transcript. Use the parameter
        /// when you want the word to appear different from its usual representation or from its spelling in training
        /// data. For example, you might indicate that the word `IBM(trademark)` is to be displayed as `IBM&trade;`.
        ///
        /// If you add a custom word that already exists in the words resource for the custom model, the new definition
        /// overwrites the existing data for the word. If the service encounters an error, it does not add the word to
        /// the words resource. Use the **List a custom word** method to review the word that you add.
        ///
        /// **See also:**
        /// * [Working with custom
        /// words](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#workingWords)
        /// * [Add words to the custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-create.html#addWords).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="wordName">The custom word that is to be added to or updated in the custom language model. Do
        /// not include spaces in the word. Use a `-` (dash) or `_` (underscore) to connect the tokens of compound
        /// words. URL-encode the word if it includes non-ASCII characters. For more information, see [Character
        /// encoding](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#charEncoding).</param>
        /// <param name="word">For the **Add custom words** method, you must specify the custom word that is to be added
        /// to or updated in the custom model. Do not include spaces in the word. Use a `-` (dash) or `_` (underscore)
        /// to connect the tokens of compound words.
        ///
        /// Omit this parameter for the **Add a custom word** method. (optional)</param>
        /// <param name="soundsLike">An array of sounds-like pronunciations for the custom word. Specify how words that
        /// are difficult to pronounce, foreign words, acronyms, and so on can be pronounced by users.
        /// * For a word that is not in the service's base vocabulary, omit the parameter to have the service
        /// automatically generate a sounds-like pronunciation for the word.
        /// * For a word that is in the service's base vocabulary, use the parameter to specify additional
        /// pronunciations for the word. You cannot override the default pronunciation of a word; pronunciations you add
        /// augment the pronunciation from the base vocabulary.
        ///
        /// A word can have at most five sounds-like pronunciations. A pronunciation can include at most 40 characters
        /// not including spaces. (optional)</param>
        /// <param name="displayAs">An alternative spelling for the custom word when it appears in a transcript. Use the
        /// parameter when you want the word to have a spelling that is different from its usual representation or from
        /// its spelling in corpora training data. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool AddWord(Callback<object> callback, string customizationId, string wordName, Dictionary<string, object> customData = null, string word = null, List<string> soundsLike = null, string displayAs = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddWord`");
            if (string.IsNullOrEmpty(wordName))
                throw new ArgumentNullException("`wordName` is required for `AddWord`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPUT,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "AddWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(word))
                bodyObject["word"] = word;
            if (soundsLike != null && soundsLike.Count > 0)
                bodyObject["sounds_like"] = JToken.FromObject(soundsLike);
            if (!string.IsNullOrEmpty(displayAs))
                bodyObject["display_as"] = displayAs;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, wordName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnAddWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add custom words.
        ///
        /// Adds one or more custom words to a custom language model. The service populates the words resource for a
        /// custom model with out-of-vocabulary (OOV) words from each corpus or grammar that is added to the model. You
        /// can use this method to add additional words or to modify existing words in the words resource. The words
        /// resource for a model can contain a maximum of 30 thousand custom (OOV) words. This includes words that the
        /// service extracts from corpora and grammars and words that you add directly.
        ///
        /// You must use credentials for the instance of the service that owns a model to add or modify custom words for
        /// the model. Adding or modifying custom words does not affect the custom model until you train the model for
        /// the new data by using the **Train a custom language model** method.
        ///
        /// You add custom words by providing a `CustomWords` object, which is an array of `CustomWord` objects, one per
        /// word. You must use the object's `word` parameter to identify the word that is to be added. You can also
        /// provide one or both of the optional `sounds_like` and `display_as` fields for each word.
        /// * The `sounds_like` field provides an array of one or more pronunciations for the word. Use the parameter to
        /// specify how the word can be pronounced by users. Use the parameter for words that are difficult to
        /// pronounce, foreign words, acronyms, and so on. For example, you might specify that the word `IEEE` can sound
        /// like `i triple e`. You can specify a maximum of five sounds-like pronunciations for a word.
        /// * The `display_as` field provides a different way of spelling the word in a transcript. Use the parameter
        /// when you want the word to appear different from its usual representation or from its spelling in training
        /// data. For example, you might indicate that the word `IBM(trademark)` is to be displayed as `IBM&trade;`.
        ///
        /// If you add a custom word that already exists in the words resource for the custom model, the new definition
        /// overwrites the existing data for the word. If the service encounters an error with the input data, it
        /// returns a failure code and does not add any of the words to the words resource.
        ///
        /// The call returns an HTTP 201 response code if the input data is valid. It then asynchronously processes the
        /// words to add them to the model's words resource. The time that it takes for the analysis to complete depends
        /// on the number of new words that you add but is generally faster than adding a corpus or grammar.
        ///
        /// You can monitor the status of the request by using the **List a custom language model** method to poll the
        /// model's status. Use a loop to check the status every 10 seconds. The method returns a `Customization` object
        /// that includes a `status` field. A status of `ready` means that the words have been added to the custom
        /// model. The service cannot accept requests to add new data or to train the model until the existing request
        /// completes.
        ///
        /// You can use the **List custom words** or **List a custom word** method to review the words that you add.
        /// Words with an invalid `sounds_like` field include an `error` field that describes the problem. You can use
        /// other words-related methods to correct errors, eliminate typos, and modify how words are pronounced as
        /// needed.
        ///
        /// **See also:**
        /// * [Working with custom
        /// words](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#workingWords)
        /// * [Add words to the custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-create.html#addWords).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="words">An array of `CustomWord` objects that provides information about each custom word that
        /// is to be added to or updated in the custom language model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool AddWords(Callback<object> callback, string customizationId, List<CustomWord> words, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddWords`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddWords`");
            if (words == null)
                throw new ArgumentNullException("`words` is required for `AddWords`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "AddWords"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (words != null && words.Count > 0)
                bodyObject["words"] = JToken.FromObject(words);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddWordsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnAddWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom word.
        ///
        /// Deletes a custom word from a custom language model. You can remove any word that you added to the custom
        /// model's words resource via any means. However, if the word also exists in the service's base vocabulary, the
        /// service removes only the custom pronunciation for the word; the word remains in the base vocabulary.
        /// Removing a custom word does not affect the custom model until you train the model with the **Train a custom
        /// language model** method. You must use credentials for the instance of the service that owns a model to
        /// delete its words.
        ///
        /// **See also:** [Deleting a word from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-words.html#deleteWord).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="wordName">The custom word that is to be deleted from the custom language model. URL-encode the
        /// word if it includes non-ASCII characters. For more information, see [Character
        /// encoding](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#charEncoding).</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteWord(Callback<object> callback, string customizationId, string wordName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteWord`");
            if (string.IsNullOrEmpty(wordName))
                throw new ArgumentNullException("`wordName` is required for `DeleteWord`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, wordName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a custom word.
        ///
        /// Gets information about a custom word from a custom language model. You must use credentials for the instance
        /// of the service that owns a model to list information about its words.
        ///
        /// **See also:** [Listing words from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-words.html#listWords).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="wordName">The custom word that is to be read from the custom language model. URL-encode the
        /// word if it includes non-ASCII characters. For more information, see [Character
        /// encoding](https://cloud.ibm.com/docs/services/speech-to-text/language-resource.html#charEncoding).</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Word" />Word</returns>
        public bool GetWord(Callback<Word> callback, string customizationId, string wordName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetWord`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetWord`");
            if (string.IsNullOrEmpty(wordName))
                throw new ArgumentNullException("`wordName` is required for `GetWord`");

            RequestObject<Word> req = new RequestObject<Word>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetWord"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetWordResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words/{1}", customizationId, wordName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetWordResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Word> response = new DetailedResponse<Word>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Word>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetWordResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Word>)req).Callback != null)
                ((RequestObject<Word>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List custom words.
        ///
        /// Lists information about custom words from a custom language model. You can list all words from the custom
        /// model's words resource, only custom words that were added or modified by the user, or only out-of-vocabulary
        /// (OOV) words that were extracted from corpora or are recognized by grammars. You can also indicate the order
        /// in which the service is to return words; by default, the service lists words in ascending alphabetical
        /// order. You must use credentials for the instance of the service that owns a model to list information about
        /// its words.
        ///
        /// **See also:** [Listing words from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/language-words.html#listWords).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="wordType">The type of words to be listed from the custom language model's words resource:
        /// * `all` (the default) shows all words.
        /// * `user` shows only custom words that were added or modified by the user directly.
        /// * `corpora` shows only OOV that were extracted from corpora.
        /// * `grammars` shows only OOV words that are recognized by grammars. (optional, default to all)</param>
        /// <param name="sort">Indicates the order in which the words are to be listed, `alphabetical` or by `count`.
        /// You can prepend an optional `+` or `-` to an argument to indicate whether the results are to be sorted in
        /// ascending or descending order. By default, words are sorted in ascending alphabetical order. For
        /// alphabetical ordering, the lexicographical precedence is numeric values, uppercase letters, and lowercase
        /// letters. For count ordering, values with the same count are ordered alphabetically. With the `curl` command,
        /// URL encode the `+` symbol as `%2B`. (optional, default to alphabetical)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Words" />Words</returns>
        public bool ListWords(Callback<Words> callback, string customizationId, Dictionary<string, object> customData = null, string wordType = null, string sort = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListWords`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListWords`");

            RequestObject<Words> req = new RequestObject<Words>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListWords"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(wordType))
            {
                req.Parameters["word_type"] = wordType;
            }
            if (!string.IsNullOrEmpty(sort))
            {
                req.Parameters["sort"] = sort;
            }

            req.OnResponse = OnListWordsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/words", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListWordsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Words> response = new DetailedResponse<Words>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Words>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListWordsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Words>)req).Callback != null)
                ((RequestObject<Words>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a grammar.
        ///
        /// Adds a single grammar file to a custom language model. Submit a plain text file in UTF-8 format that defines
        /// the grammar. Use multiple requests to submit multiple grammar files. You must use credentials for the
        /// instance of the service that owns a model to add a grammar to it. Adding a grammar does not affect the
        /// custom language model until you train the model for the new data by using the **Train a custom language
        /// model** method.
        ///
        /// The call returns an HTTP 201 response code if the grammar is valid. The service then asynchronously
        /// processes the contents of the grammar and automatically extracts new words that it finds. This can take a
        /// few seconds to complete depending on the size and complexity of the grammar, as well as the current load on
        /// the service. You cannot submit requests to add additional resources to the custom model or to train the
        /// model until the service's analysis of the grammar for the current request completes. Use the **Get a
        /// grammar** method to check the status of the analysis.
        ///
        /// The service populates the model's words resource with any word that is recognized by the grammar that is not
        /// found in the model's base vocabulary. These are referred to as out-of-vocabulary (OOV) words. You can use
        /// the **List custom words** method to examine the words resource and use other words-related methods to
        /// eliminate typos and modify how words are pronounced as needed.
        ///
        /// To add a grammar that has the same name as an existing grammar, set the `allow_overwrite` parameter to
        /// `true`; otherwise, the request fails. Overwriting an existing grammar causes the service to process the
        /// grammar file and extract OOV words anew. Before doing so, it removes any OOV words associated with the
        /// existing grammar from the model's words resource unless they were also added by another resource or they
        /// have been modified in some way with the **Add custom words** or **Add a custom word** method.
        ///
        /// The service limits the overall amount of data that you can add to a custom model to a maximum of 10 million
        /// total words from all sources combined. Also, you can add no more than 30 thousand OOV words to a model. This
        /// includes words that the service extracts from corpora and grammars and words that you add directly.
        ///
        /// **See also:**
        /// * [Working with grammars](https://cloud.ibm.com/docs/services/speech-to-text/)
        /// * [Add grammars to the custom language model](https://cloud.ibm.com/docs/services/speech-to-text/).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="grammarName">The name of the new grammar for the custom language model. Use a localized name
        /// that matches the language of the custom model and reflects the contents of the grammar.
        /// * Include a maximum of 128 characters in the name.
        /// * Do not include spaces, slashes, or backslashes in the name.
        /// * Do not use the name of an existing grammar or corpus that is already defined for the custom model.
        /// * Do not use the name `user`, which is reserved by the service to denote custom words that are added or
        /// modified by the user.</param>
        /// <param name="grammarFile">A plain text file that contains the grammar in the format specified by the
        /// `Content-Type` header. Encode the file in UTF-8 (ASCII is a subset of UTF-8). Using any other encoding can
        /// lead to issues when compiling the grammar or to unexpected results in decoding. The service ignores an
        /// encoding that is specified in the header of the grammar.</param>
        /// <param name="contentType">The format (MIME type) of the grammar file:
        /// * `application/srgs` for Augmented Backus-Naur Form (ABNF), which uses a plain-text representation that is
        /// similar to traditional BNF grammars.
        /// * `application/srgs+xml` for XML Form, which uses XML elements to represent the grammar.</param>
        /// <param name="allowOverwrite">If `true`, the specified grammar overwrites an existing grammar with the same
        /// name. If `false`, the request fails if a grammar with the same name already exists. The parameter has no
        /// effect if a grammar with the same name does not already exist. (optional, default to false)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool AddGrammar(Callback<object> callback, string customizationId, string grammarName, string grammarFile, string contentType, Dictionary<string, object> customData = null, bool? allowOverwrite = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddGrammar`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddGrammar`");
            if (string.IsNullOrEmpty(grammarName))
                throw new ArgumentNullException("`grammarName` is required for `AddGrammar`");
            if (string.IsNullOrEmpty(grammarFile))
                throw new ArgumentNullException("`grammarFile` is required for `AddGrammar`");
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("`contentType` is required for `AddGrammar`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "AddGrammar"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (allowOverwrite != null)
            {
                req.Parameters["allow_overwrite"] = (bool)allowOverwrite ? "true" : "false";
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }
            req.Send = Encoding.UTF8.GetBytes(grammarFile);

            req.OnResponse = OnAddGrammarResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/grammars/{1}", customizationId, grammarName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddGrammarResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnAddGrammarResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a grammar.
        ///
        /// Deletes an existing grammar from a custom language model. The service removes any out-of-vocabulary (OOV)
        /// words associated with the grammar from the custom model's words resource unless they were also added by
        /// another resource or they were modified in some way with the **Add custom words** or **Add a custom word**
        /// method. Removing a grammar does not affect the custom model until you train the model with the **Train a
        /// custom language model** method. You must use credentials for the instance of the service that owns a model
        /// to delete its grammar.
        ///
        /// **See also:** [Deleting a grammar from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="grammarName">The name of the grammar for the custom language model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteGrammar(Callback<object> callback, string customizationId, string grammarName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteGrammar`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteGrammar`");
            if (string.IsNullOrEmpty(grammarName))
                throw new ArgumentNullException("`grammarName` is required for `DeleteGrammar`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteGrammar"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteGrammarResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/grammars/{1}", customizationId, grammarName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteGrammarResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteGrammarResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a grammar.
        ///
        /// Gets information about a grammar from a custom language model. The information includes the total number of
        /// out-of-vocabulary (OOV) words, name, and status of the grammar. You must use credentials for the instance of
        /// the service that owns a model to list its grammars.
        ///
        /// **See also:** [Listing grammars from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="grammarName">The name of the grammar for the custom language model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Grammar" />Grammar</returns>
        public bool GetGrammar(Callback<Grammar> callback, string customizationId, string grammarName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetGrammar`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetGrammar`");
            if (string.IsNullOrEmpty(grammarName))
                throw new ArgumentNullException("`grammarName` is required for `GetGrammar`");

            RequestObject<Grammar> req = new RequestObject<Grammar>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetGrammar"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetGrammarResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/grammars/{1}", customizationId, grammarName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetGrammarResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Grammar> response = new DetailedResponse<Grammar>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Grammar>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetGrammarResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Grammar>)req).Callback != null)
                ((RequestObject<Grammar>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List grammars.
        ///
        /// Lists information about all grammars from a custom language model. The information includes the total number
        /// of out-of-vocabulary (OOV) words, name, and status of each grammar. You must use credentials for the
        /// instance of the service that owns a model to list its grammars.
        ///
        /// **See also:** [Listing grammars from a custom language
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom language model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Grammars" />Grammars</returns>
        public bool ListGrammars(Callback<Grammars> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListGrammars`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListGrammars`");

            RequestObject<Grammars> req = new RequestObject<Grammars>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListGrammars"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListGrammarsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/customizations/{0}/grammars", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListGrammarsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Grammars> response = new DetailedResponse<Grammars>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Grammars>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListGrammarsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Grammars>)req).Callback != null)
                ((RequestObject<Grammars>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a custom acoustic model.
        ///
        /// Creates a new custom acoustic model for a specified base model. The custom acoustic model can be used only
        /// with the base model for which it is created. The model is owned by the instance of the service whose
        /// credentials are used to create it.
        ///
        /// **See also:** [Create a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-create.html#createModel-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">A user-defined name for the new custom acoustic model. Use a name that is unique among
        /// all custom acoustic models that you own. Use a localized name that matches the language of the custom model.
        /// Use a name that describes the acoustic environment of the custom model, such as `Mobile custom model` or
        /// `Noisy car custom model`.</param>
        /// <param name="baseModelName">The name of the base language model that is to be customized by the new custom
        /// acoustic model. The new custom model can be used only with the base model that it customizes.
        ///
        /// To determine whether a base model supports acoustic model customization, refer to [Language support for
        /// customization](https://cloud.ibm.com/docs/services/speech-to-text/custom.html#languageSupport).</param>
        /// <param name="description">A description of the new custom acoustic model. Use a localized description that
        /// matches the language of the custom model. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="AcousticModel" />AcousticModel</returns>
        public bool CreateAcousticModel(Callback<AcousticModel> callback, string name, string baseModelName, Dictionary<string, object> customData = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateAcousticModel`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateAcousticModel`");
            if (string.IsNullOrEmpty(baseModelName))
                throw new ArgumentNullException("`baseModelName` is required for `CreateAcousticModel`");

            RequestObject<AcousticModel> req = new RequestObject<AcousticModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "CreateAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(baseModelName))
                bodyObject["base_model_name"] = baseModelName;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/acoustic_customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AcousticModel> response = new DetailedResponse<AcousticModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AcousticModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnCreateAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AcousticModel>)req).Callback != null)
                ((RequestObject<AcousticModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a custom acoustic model.
        ///
        /// Deletes an existing custom acoustic model. The custom model cannot be deleted if another request, such as
        /// adding an audio resource to the model, is currently being processed. You must use credentials for the
        /// instance of the service that owns a model to delete it.
        ///
        /// **See also:** [Deleting a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-models.html#deleteModel-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteAcousticModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteAcousticModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteAcousticModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a custom acoustic model.
        ///
        /// Gets information about a specified custom acoustic model. You must use credentials for the instance of the
        /// service that owns a model to list information about it.
        ///
        /// **See also:** [Listing custom acoustic
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-models.html#listModels-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="AcousticModel" />AcousticModel</returns>
        public bool GetAcousticModel(Callback<AcousticModel> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetAcousticModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetAcousticModel`");

            RequestObject<AcousticModel> req = new RequestObject<AcousticModel>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AcousticModel> response = new DetailedResponse<AcousticModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AcousticModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AcousticModel>)req).Callback != null)
                ((RequestObject<AcousticModel>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List custom acoustic models.
        ///
        /// Lists information about all custom acoustic models that are owned by an instance of the service. Use the
        /// `language` parameter to see all custom acoustic models for the specified language. Omit the parameter to see
        /// all custom acoustic models for all languages. You must use credentials for the instance of the service that
        /// owns a model to list information about it.
        ///
        /// **See also:** [Listing custom acoustic
        /// models](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-models.html#listModels-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="language">The identifier of the language for which custom language or custom acoustic models
        /// are to be returned (for example, `en-US`). Omit the parameter to see all custom language or custom acoustic
        /// models that are owned by the requesting credentials. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="AcousticModels" />AcousticModels</returns>
        public bool ListAcousticModels(Callback<AcousticModels> callback, Dictionary<string, object> customData = null, string language = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListAcousticModels`");

            RequestObject<AcousticModels> req = new RequestObject<AcousticModels>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListAcousticModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(language))
            {
                req.Parameters["language"] = language;
            }

            req.OnResponse = OnListAcousticModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/acoustic_customizations");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListAcousticModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AcousticModels> response = new DetailedResponse<AcousticModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AcousticModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListAcousticModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AcousticModels>)req).Callback != null)
                ((RequestObject<AcousticModels>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Reset a custom acoustic model.
        ///
        /// Resets a custom acoustic model by removing all audio resources from the model. Resetting a custom acoustic
        /// model initializes the model to its state when it was first created. Metadata such as the name and language
        /// of the model are preserved, but the model's audio resources are removed and must be re-created. You must use
        /// credentials for the instance of the service that owns a model to reset it.
        ///
        /// **See also:** [Resetting a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-models.html#resetModel-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool ResetAcousticModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ResetAcousticModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ResetAcousticModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ResetAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnResetAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/reset", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnResetAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnResetAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Train a custom acoustic model.
        ///
        /// Initiates the training of a custom acoustic model with new or changed audio resources. After adding or
        /// deleting audio resources for a custom acoustic model, use this method to begin the actual training of the
        /// model on the latest audio data. The custom acoustic model does not reflect its changed data until you train
        /// it. You must use credentials for the instance of the service that owns a model to train it.
        ///
        /// The training method is asynchronous. It can take on the order of minutes or hours to complete depending on
        /// the total amount of audio data on which the custom acoustic model is being trained and the current load on
        /// the service. Typically, training a custom acoustic model takes approximately two to four times the length of
        /// its audio data. The range of time depends on the model being trained and the nature of the audio, such as
        /// whether the audio is clean or noisy. The method returns an HTTP 200 response code to indicate that the
        /// training process has begun.
        ///
        /// You can monitor the status of the training by using the **Get a custom acoustic model** method to poll the
        /// model's status. Use a loop to check the status once a minute. The method returns an `AcousticModel` object
        /// that includes `status` and `progress` fields. A status of `available` indicates that the custom model is
        /// trained and ready to use. The service cannot accept subsequent training requests, or requests to add new
        /// audio resources, until the existing request completes.
        ///
        /// You can use the optional `custom_language_model_id` parameter to specify the GUID of a separately created
        /// custom language model that is to be used during training. Train with a custom language model if you have
        /// verbatim transcriptions of the audio files that you have added to the custom model or you have either
        /// corpora (text files) or a list of words that are relevant to the contents of the audio files. Both of the
        /// custom models must be based on the same version of the same base model for training to succeed.
        ///
        /// Training can fail to start for the following reasons:
        /// * The service is currently handling another request for the custom model, such as another training request
        /// or a request to add audio resources to the model.
        /// * The custom model contains less than 10 minutes or more than 100 hours of audio data.
        /// * One or more of the custom model's audio resources is invalid.
        /// * You passed an incompatible custom language model with the `custom_language_model_id` query parameter. Both
        /// custom models must be based on the same version of the same base model.
        ///
        /// **See also:** [Train the custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-create.html#trainModel-acoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customLanguageModelId">The customization ID (GUID) of a custom language model that is to be
        /// used during training of the custom acoustic model. Specify a custom language model that has been trained
        /// with verbatim transcriptions of the audio resources or that contains words that are relevant to the contents
        /// of the audio resources. The custom language model must be based on the same version of the same base model
        /// as the custom acoustic model. The credentials specified with the request must own both custom models.
        /// (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool TrainAcousticModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null, string customLanguageModelId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `TrainAcousticModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `TrainAcousticModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "TrainAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(customLanguageModelId))
            {
                req.Parameters["custom_language_model_id"] = customLanguageModelId;
            }

            req.OnResponse = OnTrainAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/train", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnTrainAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnTrainAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Upgrade a custom acoustic model.
        ///
        /// Initiates the upgrade of a custom acoustic model to the latest version of its base language model. The
        /// upgrade method is asynchronous. It can take on the order of minutes or hours to complete depending on the
        /// amount of data in the custom model and the current load on the service; typically, upgrade takes
        /// approximately twice the length of the total audio contained in the custom model. A custom model must be in
        /// the `ready` or `available` state to be upgraded. You must use credentials for the instance of the service
        /// that owns a model to upgrade it.
        ///
        /// The method returns an HTTP 200 response code to indicate that the upgrade process has begun successfully.
        /// You can monitor the status of the upgrade by using the **Get a custom acoustic model** method to poll the
        /// model's status. The method returns an `AcousticModel` object that includes `status` and `progress` fields.
        /// Use a loop to check the status once a minute. While it is being upgraded, the custom model has the status
        /// `upgrading`. When the upgrade is complete, the model resumes the status that it had prior to upgrade. The
        /// service cannot accept subsequent requests for the model until the upgrade completes.
        ///
        /// If the custom acoustic model was trained with a separately created custom language model, you must use the
        /// `custom_language_model_id` parameter to specify the GUID of that custom language model. The custom language
        /// model must be upgraded before the custom acoustic model can be upgraded. Omit the parameter if the custom
        /// acoustic model was not trained with a custom language model.
        ///
        /// **See also:** [Upgrading a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/custom-upgrade.html#upgradeAcoustic).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customLanguageModelId">If the custom acoustic model was trained with a custom language model,
        /// the customization ID (GUID) of that custom language model. The custom language model must be upgraded before
        /// the custom acoustic model can be upgraded. The credentials specified with the request must own both custom
        /// models. (optional)</param>
        /// <param name="force">If `true`, forces the upgrade of a custom acoustic model for which no input data has
        /// been modified since it was last trained. Use this parameter only to force the upgrade of a custom acoustic
        /// model that is trained with a custom language model, and only if you receive a 400 response code and the
        /// message `No input data modified since last training`. See [Upgrading a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/custom-upgrade.html#upgradeAcoustic). (optional,
        /// default to false)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool UpgradeAcousticModel(Callback<object> callback, string customizationId, Dictionary<string, object> customData = null, string customLanguageModelId = null, bool? force = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpgradeAcousticModel`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `UpgradeAcousticModel`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "UpgradeAcousticModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(customLanguageModelId))
            {
                req.Parameters["custom_language_model_id"] = customLanguageModelId;
            }
            if (force != null)
            {
                req.Parameters["force"] = (bool)force ? "true" : "false";
            }

            req.OnResponse = OnUpgradeAcousticModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/upgrade_model", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpgradeAcousticModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnUpgradeAcousticModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add an audio resource.
        ///
        /// Adds an audio resource to a custom acoustic model. Add audio content that reflects the acoustic
        /// characteristics of the audio that you plan to transcribe. You must use credentials for the instance of the
        /// service that owns a model to add an audio resource to it. Adding audio data does not affect the custom
        /// acoustic model until you train the model for the new data by using the **Train a custom acoustic model**
        /// method.
        ///
        /// You can add individual audio files or an archive file that contains multiple audio files. Adding multiple
        /// audio files via a single archive file is significantly more efficient than adding each file individually.
        /// You can add audio resources in any format that the service supports for speech recognition.
        ///
        /// You can use this method to add any number of audio resources to a custom model by calling the method once
        /// for each audio or archive file. But the addition of one audio resource must be fully complete before you can
        /// add another. You must add a minimum of 10 minutes and a maximum of 100 hours of audio that includes speech,
        /// not just silence, to a custom acoustic model before you can train it. No audio resource, audio- or
        /// archive-type, can be larger than 100 MB. To add an audio resource that has the same name as an existing
        /// audio resource, set the `allow_overwrite` parameter to `true`; otherwise, the request fails.
        ///
        /// The method is asynchronous. It can take several seconds to complete depending on the duration of the audio
        /// and, in the case of an archive file, the total number of audio files being processed. The service returns a
        /// 201 response code if the audio is valid. It then asynchronously analyzes the contents of the audio file or
        /// files and automatically extracts information about the audio such as its length, sampling rate, and
        /// encoding. You cannot submit requests to add additional audio resources to a custom acoustic model, or to
        /// train the model, until the service's analysis of all audio files for the current request completes.
        ///
        /// To determine the status of the service's analysis of the audio, use the **Get an audio resource** method to
        /// poll the status of the audio. The method accepts the customization ID of the custom model and the name of
        /// the audio resource, and it returns the status of the resource. Use a loop to check the status of the audio
        /// every few seconds until it becomes `ok`.
        ///
        /// **See also:** [Add audio to the custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-create.html#addAudio).
        ///
        /// ### Content types for audio-type resources
        ///
        ///  You can add an individual audio file in any format that the service supports for speech recognition. For an
        /// audio-type resource, use the `Content-Type` parameter to specify the audio format (MIME type) of the audio
        /// file, including specifying the sampling rate, channels, and endianness where indicated.
        /// * `audio/alaw` (Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/basic` (Use only with narrowband models.)
        /// * `audio/flac`
        /// * `audio/g729` (Use only with narrowband models.)
        /// * `audio/l16` (Specify the sampling rate (`rate`) and optionally the number of channels (`channels`) and
        /// endianness (`endianness`) of the audio.)
        /// * `audio/mp3`
        /// * `audio/mpeg`
        /// * `audio/mulaw` (Specify the sampling rate (`rate`) of the audio.)
        /// * `audio/ogg` (The service automatically detects the codec of the input audio.)
        /// * `audio/ogg;codecs=opus`
        /// * `audio/ogg;codecs=vorbis`
        /// * `audio/wav` (Provide audio with a maximum of nine channels.)
        /// * `audio/webm` (The service automatically detects the codec of the input audio.)
        /// * `audio/webm;codecs=opus`
        /// * `audio/webm;codecs=vorbis`
        ///
        /// The sampling rate of an audio file must match the sampling rate of the base model for the custom model: for
        /// broadband models, at least 16 kHz; for narrowband models, at least 8 kHz. If the sampling rate of the audio
        /// is higher than the minimum required rate, the service down-samples the audio to the appropriate rate. If the
        /// sampling rate of the audio is lower than the minimum required rate, the service labels the audio file as
        /// `invalid`.
        ///
        ///  **See also:** [Audio formats](https://cloud.ibm.com/docs/services/speech-to-text/audio-formats.html).
        ///
        /// ### Content types for archive-type resources
        ///
        ///  You can add an archive file (**.zip** or **.tar.gz** file) that contains audio files in any format that the
        /// service supports for speech recognition. For an archive-type resource, use the `Content-Type` parameter to
        /// specify the media type of the archive file:
        /// * `application/zip` for a **.zip** file
        /// * `application/gzip` for a **.tar.gz** file.
        ///
        /// All audio files contained in the archive must have the same audio format. Use the `Contained-Content-Type`
        /// parameter to specify the format of the contained audio files. The parameter accepts all of the audio formats
        /// supported for use with speech recognition and with the `Content-Type` header, including the `rate`,
        /// `channels`, and `endianness` parameters that are used with some formats. The default contained audio format
        /// is `audio/wav`.
        ///
        /// ### Naming restrictions for embedded audio files
        ///
        ///  The name of an audio file that is embedded within an archive-type resource must meet the following
        /// restrictions:
        /// * Include a maximum of 128 characters in the file name; this includes the file extension.
        /// * Do not include spaces, slashes, or backslashes in the file name.
        /// * Do not use the name of an audio file that has already been added to the custom model as part of an
        /// archive-type resource.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="audioName">The name of the new audio resource for the custom acoustic model. Use a localized
        /// name that matches the language of the custom model and reflects the contents of the resource.
        /// * Include a maximum of 128 characters in the name.
        /// * Do not include spaces, slashes, or backslashes in the name.
        /// * Do not use the name of an audio resource that has already been added to the custom model.</param>
        /// <param name="audioResource">The audio resource that is to be added to the custom acoustic model, an
        /// individual audio file or an archive file.</param>
        /// <param name="containedContentType">For an archive-type resource, specifies the format of the audio files
        /// that are contained in the archive file. The parameter accepts all of the audio formats that are supported
        /// for use with speech recognition, including the `rate`, `channels`, and `endianness` parameters that are used
        /// with some formats. For more information, see **Content types for audio-type resources** in the method
        /// description. (optional, default to audio/wav)</param>
        /// <param name="allowOverwrite">If `true`, the specified audio resource overwrites an existing audio resource
        /// with the same name. If `false`, the request fails if an audio resource with the same name already exists.
        /// The parameter has no effect if an audio resource with the same name does not already exist. (optional,
        /// default to false)</param>
        /// <param name="contentType">For an audio-type resource, the format (MIME type) of the audio. For more
        /// information, see **Content types for audio-type resources** in the method description.
        ///
        /// For an archive-type resource, the media type of the archive file. For more information, see **Content types
        /// for archive-type resources** in the method description. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool AddAudio(Callback<object> callback, string customizationId, string audioName, byte[] audioResource, Dictionary<string, object> customData = null, string containedContentType = null, bool? allowOverwrite = null, string contentType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddAudio`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `AddAudio`");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("`audioName` is required for `AddAudio`");
            if (audioResource == null)
                throw new ArgumentNullException("`audioResource` is required for `AddAudio`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "AddAudio"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (allowOverwrite != null)
            {
                req.Parameters["allow_overwrite"] = (bool)allowOverwrite ? "true" : "false";
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(containedContentType))
            {
                req.Headers["Contained-Content-Type"] = containedContentType;
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }
            req.Send = audioResource;

            req.OnResponse = OnAddAudioResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio/{1}", customizationId, audioName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnAddAudioResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnAddAudioResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete an audio resource.
        ///
        /// Deletes an existing audio resource from a custom acoustic model. Deleting an archive-type audio resource
        /// removes the entire archive of files; the current interface does not allow deletion of individual files from
        /// an archive resource. Removing an audio resource does not affect the custom model until you train the model
        /// on its updated data by using the **Train a custom acoustic model** method. You must use credentials for the
        /// instance of the service that owns a model to delete its audio resources.
        ///
        /// **See also:** [Deleting an audio resource from a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-audio.html#deleteAudio).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="audioName">The name of the audio resource for the custom acoustic model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteAudio(Callback<object> callback, string customizationId, string audioName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteAudio`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `DeleteAudio`");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("`audioName` is required for `DeleteAudio`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteAudio"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnDeleteAudioResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio/{1}", customizationId, audioName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteAudioResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteAudioResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get an audio resource.
        ///
        /// Gets information about an audio resource from a custom acoustic model. The method returns an `AudioListing`
        /// object whose fields depend on the type of audio resource that you specify with the method's `audio_name`
        /// parameter:
        /// * **For an audio-type resource,** the object's fields match those of an `AudioResource` object: `duration`,
        /// `name`, `details`, and `status`.
        /// * **For an archive-type resource,** the object includes a `container` field whose fields match those of an
        /// `AudioResource` object. It also includes an `audio` field, which contains an array of `AudioResource`
        /// objects that provides information about the audio files that are contained in the archive.
        ///
        /// The information includes the status of the specified audio resource. The status is important for checking
        /// the service's analysis of a resource that you add to the custom model.
        /// * For an audio-type resource, the `status` field is located in the `AudioListing` object.
        /// * For an archive-type resource, the `status` field is located in the `AudioResource` object that is returned
        /// in the `container` field.
        ///
        /// You must use credentials for the instance of the service that owns a model to list its audio resources.
        ///
        /// **See also:** [Listing audio resources for a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-audio.html#listAudio).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="audioName">The name of the audio resource for the custom acoustic model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="AudioListing" />AudioListing</returns>
        public bool GetAudio(Callback<AudioListing> callback, string customizationId, string audioName, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetAudio`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `GetAudio`");
            if (string.IsNullOrEmpty(audioName))
                throw new ArgumentNullException("`audioName` is required for `GetAudio`");

            RequestObject<AudioListing> req = new RequestObject<AudioListing>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "GetAudio"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnGetAudioResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio/{1}", customizationId, audioName));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetAudioResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AudioListing> response = new DetailedResponse<AudioListing>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AudioListing>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnGetAudioResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AudioListing>)req).Callback != null)
                ((RequestObject<AudioListing>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List audio resources.
        ///
        /// Lists information about all audio resources from a custom acoustic model. The information includes the name
        /// of the resource and information about its audio data, such as its duration. It also includes the status of
        /// the audio resource, which is important for checking the service's analysis of the resource in response to a
        /// request to add it to the custom acoustic model. You must use credentials for the instance of the service
        /// that owns a model to list its audio resources.
        ///
        /// **See also:** [Listing audio resources for a custom acoustic
        /// model](https://cloud.ibm.com/docs/services/speech-to-text/acoustic-audio.html#listAudio).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customizationId">The customization ID (GUID) of the custom acoustic model that is to be used
        /// for the request. You must make the request with credentials for the instance of the service that owns the
        /// custom model.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="AudioResources" />AudioResources</returns>
        public bool ListAudio(Callback<AudioResources> callback, string customizationId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListAudio`");
            if (string.IsNullOrEmpty(customizationId))
                throw new ArgumentNullException("`customizationId` is required for `ListAudio`");

            RequestObject<AudioResources> req = new RequestObject<AudioResources>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "ListAudio"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }


            req.OnResponse = OnListAudioResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v1/acoustic_customizations/{0}/audio", customizationId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListAudioResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AudioResources> response = new DetailedResponse<AudioResources>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AudioResources>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnListAudioResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AudioResources>)req).Callback != null)
                ((RequestObject<AudioResources>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data that is associated with a specified customer ID. The method deletes all data for the
        /// customer ID, regardless of the method by which the information was added. The method has no effect if no
        /// data is associated with the customer ID. You must issue the request with credentials for the same instance
        /// of the service that was used to associate the customer ID with the data.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// the data.
        ///
        /// **See also:** [Information
        /// security](https://cloud.ibm.com/docs/services/speech-to-text/information-security.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("speech_to_text", "V1", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/user_data");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("SpeechToTextService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}