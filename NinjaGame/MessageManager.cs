using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MessageManager
{
    public static MessageManager instance;
    private static readonly object padlock = new object();
    private List<Message> messages = new List<Message>();
    public int messageLine = 0;

    public MessageManager() { }

    public static MessageManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new MessageManager();
                }
                return Instance;
            }
        }
    }
    public void AddMessage(string content)
    {
        messageLine++;

        Message message = new Message(content);
        messages.Add(message);

        Console.SetCursorPosition(0, messageLine);
        message.Display();
    }
}

