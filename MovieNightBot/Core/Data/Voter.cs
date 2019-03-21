using System;
using System.Collections.Generic;
using System.Text;

namespace MovieNightBot.Core.Data {

    public class Voter {

        public Voter() {
            votes = new List<int>();
        }

        public List<int> votes;
        public int numVotes;

    }
}
