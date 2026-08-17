namespace CaptainPinkTurd.Scene
{
    public static class SceneDatabase 
    {
        //Slots = lifecycle ownership, the roles each scenes are supposed to represent
        public class Slots
        {
            public const string Menu = "Menu";
            public const string Session = "Session";
            public const string SessionContent = "SessionContent"; //the currently loaded gameplay “content” scene
        }
    }
}