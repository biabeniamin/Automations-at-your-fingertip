using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FacialRecognition
{
    public class FacialRecognitionApi
    {
        private FaceServiceClient _faceServiceClient = new FaceServiceClient(Subscription.SubscriptionKey, Subscription.Endpoint);
        private LargePersonGroup _personGroup;
        private List<Microsoft.ProjectOxford.Face.Contract.Person> _people;

        private async void GetLargePersonGroup()
        {
            try
            {
                var data = await _faceServiceClient.ListLargePersonGroupsAsync();
                if (0 == data.Length)
                {
                    _personGroup = new LargePersonGroup();
                    _personGroup.LargePersonGroupId = Guid.NewGuid().ToString();
                    await _faceServiceClient.CreateLargePersonGroupAsync(_personGroup.LargePersonGroupId, _personGroup.LargePersonGroupId);
                }
                else
                {
                    _personGroup = data[0];

                }
                UpdatePeopleList();


            }
            catch(FaceAPIException ee)
            {
                MessageBox.Show(ee.Message);
            }

        }

        private async void UpdatePeopleList()

        {
            try
            {
                var personsInGroup = await _faceServiceClient.ListPersonsInLargePersonGroupAsync(_personGroup.LargePersonGroupId);

                _people = new List<Microsoft.ProjectOxford.Face.Contract.Person>();
                foreach (var face in personsInGroup)
                {
                    _people.Add(face);
                }
            }
            catch(FaceAPIException ee)
            {
                MessageBox.Show(ee.Message);
            }
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
                    string name = "Familiar";
                    foreach (var result in identifyResult)
                    {
                        if (0 < result.Candidates.Length)
                        {
                            if (_people.Any(p => p.PersonId == result.Candidates[0].PersonId))
                            {
                                name = _people.Where(p => p.PersonId == result.Candidates[0].PersonId).First().Name;
                            }
                            wasRecognized = true;
                            break;
                        }
                    }

                    if (true == wasRecognized)
                    {
                        System.Windows.MessageBox.Show($"{name} face detected!");
                    }
                    else
                    {
                        MessageBox.Show("No familiar face detected!");
                    }
                }
                catch (FaceAPIException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return facesLocation;
        }

        public async void AddFace(String imagePath, String name)
        {
            bool hasFailed = false;
            using (var fStream = File.OpenRead(imagePath))
            {
                try
                {
                    IdentifyResult[] identifyResult = new IdentifyResult[1]; ;
                    var faces = await _faceServiceClient.DetectAsync(fStream);
                    if(0 == faces.Length)
                    {
                        MessageBox.Show("No face detected!");
                        return;
                    }

                    try
                    {
                        var personsInGroup = await _faceServiceClient.ListPersonsInLargePersonGroupAsync(_personGroup.LargePersonGroupId);
                        if (0 == personsInGroup.Length)
                        {
                            identifyResult[0] = new IdentifyResult();
                            identifyResult[0].Candidates = new Candidate[0];
                            identifyResult[0].FaceId = faces[0].FaceId;
                        }
                        else
                        {
                            identifyResult = await _faceServiceClient.IdentifyAsync(faces.Select(ff => ff.FaceId).ToArray(), largePersonGroupId: _personGroup.LargePersonGroupId);
                        }
                    }
                    catch(FaceAPIException ex)
                    {

                    }

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
                        else
                        {
                            using (var fStream2 = File.OpenRead(imagePath))
                            {
                                var res = await _faceServiceClient.AddPersonFaceInLargePersonGroupAsync(_personGroup.LargePersonGroupId,
                                result.Candidates[0].PersonId,
                                fStream2,
                                imagePath);
                            }
                        }
                    }

                }
                catch (FaceAPIException ex)
                {
                    hasFailed = true;
                    MessageBox.Show(ex.Message);
                }
            }

            await _faceServiceClient.TrainLargePersonGroupAsync(_personGroup.LargePersonGroupId);

            // Wait until train completed
            while (true)
            {
                await Task.Delay(1000);
                var status = await _faceServiceClient.GetLargePersonGroupTrainingStatusAsync(_personGroup.LargePersonGroupId);
                if (status.Status != Microsoft.ProjectOxford.Face.Contract.Status.Running)
                {
                    break;
                }
            }

            if (!hasFailed)
            {
                UpdatePeopleList();
                MessageBox.Show("The face was added and the model was trained!");
            }
        }

        public FacialRecognitionApi()
        {
            _people = new List<Microsoft.ProjectOxford.Face.Contract.Person>();
            GetLargePersonGroup();
            
        }


    }
}
