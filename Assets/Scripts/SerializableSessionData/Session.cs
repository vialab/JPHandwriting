using System;
using System.Collections.Generic;

namespace GameData {
    public record Session {
        /// <summary>
        /// The ID of the user currently running the session.
        /// </summary>
        public string UserID;

        /// <summary>
        /// The list of vocabulary items the user will write in the session.
        /// </summary>
        public List<SerializedScript<VocabItem>> VocabItems;
    }
}