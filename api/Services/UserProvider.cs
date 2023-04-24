using System.Net;
using Microsoft.EntityFrameworkCore;
using pastemyst.DbContexts;
using pastemyst.Exceptions;
using pastemyst.Models;

namespace pastemyst.Services;

public interface IUserProvider
{
    public Task<User> GetByUsernameOrIdAsync(string username, string id);

    public Task<User> GetByUsernameAsync(string username);

    public Task<bool> ExistsByUsernameAsync(string username);

    public Task<Page<PasteWithLangStats>> GetOwnedPastesAsync(string username, string tag, bool pinnedOnly, PageRequest pageRequest);

    public Task<List<string>> GetTagsAsync(string username);
}

public class UserProvider : IUserProvider
{
    private readonly IUserContext _userContext;
    private readonly IPasteService _pasteService;
    private readonly DataContext _dbContext;

    public UserProvider(DataContext dbContext, IUserContext userContext, IPasteService pasteService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _pasteService = pasteService;
    }

    public async Task<User> GetByUsernameOrIdAsync(string username, string id)
    {
        User user = null;

        if (username is not null)
        {
            user = await _dbContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }
        else if (id is not null)
        {
            user = await _dbContext.Users.FindAsync(id);
        }

        return user;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users.Include(u => u.Settings)
            .FirstOrDefaultAsync(user => user.Username.Equals(username));
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await GetByUsernameAsync(username) is not null;
    }

    public async Task<Page<PasteWithLangStats>> GetOwnedPastesAsync(string username, string tag, bool pinnedOnly, PageRequest pageRequest)
    {
        var user = await GetByUsernameAsync(username);

        // If not showing only pinned pastes, and show all pastes is disabled, return an empty list.
        if (!pinnedOnly && _userContext.Self != user && !user.Settings.ShowAllPastesOnProfile)
        {
            return new Page<PasteWithLangStats>();
        }

        var pastesQuery = _dbContext.Pastes
            .Where(p => p.Owner == user) // check owner
            .Where(p => !p.Private || p.Owner == _userContext.Self) // only get private if self is owner
            .Where(p => !pinnedOnly || p.Pinned); // if pinnedOnly, make sure all pasted are pinned

        if (tag is not null)
        {
            if (_userContext.Self != user)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "You must be authorized to view paste tags.");
            }

            pastesQuery = pastesQuery.Where(p => p.Tags.Contains(tag));
        }

        var pastes = pastesQuery.OrderBy(p => p.CreatedAt)
            .Reverse()
            .Include(p => p.Pasties)
            .Skip(pageRequest.Page * pageRequest.PageSize)
            .Take(pageRequest.PageSize)
            .ToList();

        var totalItems = pastesQuery.Count();
        var totalPages = (int)Math.Ceiling((float)totalItems / pageRequest.PageSize);

        if (_userContext.Self != user)
        {
            pastes.ForEach(p => p.Tags = new());
        }

        var pastesWithLangStags = new List<PasteWithLangStats>();
        foreach (var paste in pastes)
        {
            var stats = _pasteService.GetLanguageStats(paste);

            pastesWithLangStags.Add(new() { Paste = paste, LanguageStats = stats });
        }

        return new Page<PasteWithLangStats>
        {
            Items = pastesWithLangStags,
            CurrentPage = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            HasNextPage = pageRequest.Page < totalPages - 1,
            TotalPages = totalPages
        };
    }

    public async Task<List<string>> GetTagsAsync(string username)
    {
        if (!_userContext.IsLoggedIn())
            throw new HttpException(HttpStatusCode.Unauthorized, "You must be authorized to get your own tags.");

        var user = await GetByUsernameAsync(username);

        if (_userContext.Self != user)
            throw new HttpException(HttpStatusCode.Unauthorized, "You can only fetch your own tags.");

        return _dbContext.Pastes
            .Where(p => p.Owner == user)
            .ToList()
            .SelectMany(p => p.Tags ?? new List<string>())
            .Distinct()
            .ToList();
    }
}
