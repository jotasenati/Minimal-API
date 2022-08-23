using Flunt.Notifications;
using Flunt.Validations;

namespace MiniTodo.ViewModels
{
    public class UpdateTodoViewModel : Notifiable<Notification>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Done { get; set; }

        public Todo MapTo()
        {
            AddNotifications(new Contract<Notification>()
                .Requires()
                .IsNotNull(Id, "Informe o Id da tarefa"));

            return new Todo() { Id = Guid.NewGuid(), Title = Title, Done = false };
        }
    }
}