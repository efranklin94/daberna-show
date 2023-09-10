using DabernaShow;
using DabernaShow.Models;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void UpdateOrCreateUser(User user)
    {
        if (user.Stats == null || user == null || string.IsNullOrWhiteSpace(user.Username) || user.Score < 0)
        {
            throw new ArgumentException("Invalid user data.");
        }
        _userRepository.UpdateOrCreateUser(user);
    }

    public List<UserDTO> GetTopUsersByStats(dynamic stat)
    {
        List<UserDTO> usersDTO = new List<UserDTO>();
        var users = _userRepository.GetTopUsersByStats(stat);
        foreach (var item in users)
        {
            UserDTO userDTO = new UserDTO() 
            {
                Username = item.Username,
                Score = item.Score
            };
            usersDTO.Add(userDTO);
        }
        return usersDTO;
    }
}
