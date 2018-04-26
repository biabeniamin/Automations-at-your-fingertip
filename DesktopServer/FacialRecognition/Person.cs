using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

namespace FacialRecognition
{
    public class Person
    {
        private Face _face;
        private Microsoft.ProjectOxford.Face.Contract.Person _person;

        public Person(Face face, Microsoft.ProjectOxford.Face.Contract.Person person)
        {
            _face = face;
            this._person = person;
        }

        public Person(Face face)
        {
            _face = face;
            this._person = null;
        }

        public Microsoft.ProjectOxford.Face.Contract.Person PersonA
        {
            get { return _person; }
            set { _person = value; }
        }

        public Face Face
        {
            get { return _face; }
            set { _face = value; }
        }

    }
}
