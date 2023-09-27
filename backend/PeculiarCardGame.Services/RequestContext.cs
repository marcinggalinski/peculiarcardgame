using PeculiarCardGame.Data.Models;

namespace PeculiarCardGame.Services
{
    public class RequestContext
    {
        private bool _isSet = false;

        public User? CallingUser { get; private set; }

        public void SetOnce(User user)
        {
            if (_isSet)
                throw new InvalidOperationException($"{nameof(RequestContext)} can only be set once.");

            _isSet = true;
            CallingUser = user;
        }
    }
}
