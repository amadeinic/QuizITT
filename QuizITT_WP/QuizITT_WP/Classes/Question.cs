using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizITT_WP.Classes
{
    class Question
    {
        public int QuestionId { get; }
        public int Category { get; }
        public int Level { get; }
        public string Text { get; }
        public Answer[] Answers { get; set; }

        public Question(string riga)
        {
            string[] container;
            container = riga.Split(';');
            QuestionId = Convert.ToInt32(container[0]);
            Category = Convert.ToInt32(container[1]);
            Level = Convert.ToInt32(container[2]);
            Text = container[3];
            Answers = new Answer[3];
            Answers[0] = new Answer { Text = container[4], IsRight = Convert.ToBoolean(container[5]) };
            Answers[1] = new Answer { Text = container[6], IsRight = Convert.ToBoolean(container[7]) };
            Answers[2] = new Answer { Text = container[8], IsRight = Convert.ToBoolean(container[9]) };
        }

        public void MixAnswers()
        {

        }

    }
}
