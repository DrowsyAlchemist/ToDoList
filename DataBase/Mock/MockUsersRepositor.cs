using ToDoList.Models;

namespace ToDoList.DataBase.Mock
{
    public class MockUsersRepository
    {
        public readonly static List<UserModel> Users =
        [
            new UserModel(){Id="1", Name="Michael", LoginData=new LoginData(){Email="m.cl@gmail.com", Password ="1234" },
                Role=Role.User },
        ];
    }
}
