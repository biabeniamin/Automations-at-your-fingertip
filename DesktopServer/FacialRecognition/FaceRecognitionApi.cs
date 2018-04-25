using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecognition
{
    public class FacialRecognitionApi
    {
        private FaceServiceClient _faceServiceClient = new FaceServiceClient(Subscription.SubscriptionKey, Subscription.Endpoint);
        private LargePersonGroup _personGroup;
        private async void GetLargePersonGroup()
        {
            var data = await _faceServiceClient.ListLargePersonGroupsAsync();
            if (0 < data.Length)
            {
                _personGroup = data[0];
            }

            _personGroup.LargePersonGroupId = "1d321ce4-f343-4b92-8d68-1c97bd0ba007";
        }

        public async Task<List<Face>> DoesExists(String imagePath)
        {
            List<Face> facesLocation = new List<Face>();
            // Call detection REST API
            using (var fStream = File.OpenRead(imagePath))
            {
                try
                {
                    var faces = await _faceServiceClient.DetectAsync(fStream);
                    foreach (var face in faces)
                    {
                        facesLocation.Add(new Face(face.FaceRectangle.Left,
                            face.FaceRectangle.Top,
                            face.FaceRectangle.Width,
                            face.FaceRectangle.Height));
                    }

                    // Convert detection result into UI binding object for rendering
                    var identifyResult = await _faceServiceClient.IdentifyAsync(faces.Select(ff => ff.FaceId).ToArray(), largePersonGroupId: _personGroup.LargePersonGroupId);

                    bool wasRecognized = false;
                    foreach (var result in identifyResult)
                    {
                        if (0 < result.Candidates.Length)
                        {
                            wasRecognized = true;
                            break;
                        }
                    }

                    if (true == wasRecognized)
                    {
                        System.Windows.MessageBox.Show("Was recognized");
                    }
                }
                catch (FaceAPIException ex)
                {
                }
            }
            return facesLocation;
        }

        public async void AddFace(String imagePath, String name)
        {
            using (var fStream = File.OpenRead(imagePath))
            {
                try
                {
                    var faces = await _faceServiceClient.DetectAsync(fStream);


                    var identifyResult = await _faceServiceClient.IdentifyAsync(faces.Select(ff => ff.FaceId).ToArray(), largePersonGroupId: _personGroup.LargePersonGroupId);

                    bool wasRecognized = false;
                    foreach (var result in identifyResult)
                    {
                        if (0 == result.Candidates.Length)
                        {
                            Person person = new Person();
                            person.Name = name;
                            person.PersonId = (await _faceServiceClient.CreatePersonInLargePersonGroupAsync(_personGroup.LargePersonGroupId,
                                name)).PersonId;

                            using (var fStream2 = File.OpenRead(imagePath))
                            {
                                var res = await _faceServiceClient.AddPersonFaceInLargePersonGroupAsync(_personGroup.LargePersonGroupId,
                                person.PersonId,
                                fStream2,
                                imagePath);
                            }
                        }
                    }

                }
                catch (FaceAPIException ex)
                {
                }
            }
        }

                public FacialRecognitionApi()
        {
            GetLargePersonGroup();
        }


    }
}
