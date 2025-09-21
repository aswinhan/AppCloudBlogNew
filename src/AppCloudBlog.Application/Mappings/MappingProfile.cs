namespace AppCloudBlog.Application.Mappings;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // --- User & Authentication Mappings ---

        config.NewConfig<ApplicationUser, UserDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Bio, src => src.Bio)
            .Map(dest => dest.ProfilePictureUrl, src => src.ProfilePictureUrl)
            .Map(dest => dest.RegistrationDate, src => src.RegistrationDate)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Ignore(dest => dest.Roles);

        config.NewConfig<ApplicationUser, UserFollowDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.ProfilePictureUrl, src => src.ProfilePictureUrl);

        config.NewConfig<UserRegistrationDto, ApplicationUser>()
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PasswordHash);

        config.NewConfig<UserProfileUpdateDto, ApplicationUser>()
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.Bio, src => src.Bio)
            .Map(dest => dest.ProfilePictureUrl, src => src.ProfilePictureUrl)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Email)
            .Ignore(dest => dest.UserName);

        // --- Post Mappings ---

        config.NewConfig<Post, PostDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.Excerpt, src => src.Excerpt)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.PublishDate, src => src.PublishDate)
            .Map(dest => dest.IsPublished, src => src.IsPublished)
            .Map(dest => dest.ViewCount, src => src.ViewCount)
            .Map(dest => dest.FeaturedImageUrl, src => src.FeaturedImageUrl)
            .Map(dest => dest.Author, src => src.Author.Adapt<UserDto>())
            .Map(dest => dest.Categories, src => src.PostCategories.Select(pc => pc.Category).Adapt<ICollection<CategoryDto>>())
            .Map(dest => dest.Tags, src => src.PostTags.Select(pt => pt.Tag).Adapt<ICollection<TagDto>>())
            .Map(dest => dest.Comments, src => src.Comments.Adapt<ICollection<CommentDto>>())
            .Ignore(dest => dest.LikeCount)
            .Ignore(dest => dest.IsLikedByUser)
            .Ignore(dest => dest.IsSavedByUser);

        config.NewConfig<Post, PostListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Excerpt, src => src.Excerpt)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.PublishDate, src => src.PublishDate)
            .Map(dest => dest.IsPublished, src => src.IsPublished)
            .Map(dest => dest.ViewCount, src => src.ViewCount)
            .Map(dest => dest.FeaturedImageUrl, src => src.FeaturedImageUrl)
            .Map(dest => dest.Author, src => src.Author.Adapt<UserDto>())
            .Map(dest => dest.Categories, src => src.PostCategories.Select(pc => pc.Category).Adapt<ICollection<CategoryDto>>())
            .Map(dest => dest.Tags, src => src.PostTags.Select(pt => pt.Tag).Adapt<ICollection<TagDto>>())
            .Ignore(dest => dest.CommentCount)
            .Ignore(dest => dest.LikeCount);

        config.NewConfig<CreatePostDto, Post>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.Excerpt, src => src.Excerpt)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.FeaturedImageUrl, src => src.FeaturedImageUrl)
            .Map(dest => dest.IsPublished, src => src.IsPublished)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.AuthorId)
            .Ignore(dest => dest.Author)
            .Ignore(dest => dest.PublishDate)
            .Ignore(dest => dest.ViewCount)
            .Ignore(dest => dest.PostCategories)
            .Ignore(dest => dest.PostTags)
            .Ignore(dest => dest.Comments)
            .Ignore(dest => dest.Likes)
            .Ignore(dest => dest.SavedPosts);

        config.NewConfig<UpdatePostDto, Post>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.Excerpt, src => src.Excerpt)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.FeaturedImageUrl, src => src.FeaturedImageUrl)
            .Map(dest => dest.IsPublished, src => src.IsPublished)
            .Ignore(dest => dest.AuthorId)
            .Ignore(dest => dest.Author)
            .Ignore(dest => dest.PublishDate)
            .Ignore(dest => dest.ViewCount)
            .Ignore(dest => dest.PostCategories)
            .Ignore(dest => dest.PostTags)
            .Ignore(dest => dest.Comments)
            .Ignore(dest => dest.Likes)
            .Ignore(dest => dest.SavedPosts);

        // --- Category Mappings ---

        config.NewConfig<Category, CategoryDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.Description, src => src.Description)
            .Ignore(dest => dest.PostCount);

        config.NewConfig<CreateCategoryDto, Category>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.Description, src => src.Description)
            .Ignore(dest => dest.Id);

        config.NewConfig<UpdateCategoryDto, Category>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Map(dest => dest.Description, src => src.Description);

        // --- Tag Mappings ---

        config.NewConfig<Tag, TagDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Ignore(dest => dest.PostCount);

        config.NewConfig<CreateTagDto, Tag>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug)
            .Ignore(dest => dest.Id);

        config.NewConfig<UpdateTagDto, Tag>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Slug, src => src.Slug);

        // --- Comment Mappings ---

        config.NewConfig<Comment, CommentDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.CommentDate, src => src.CommentDate)
            .Map(dest => dest.IsApproved, src => src.IsApproved)
            .Map(dest => dest.Commenter, src => src.Commenter.Adapt<UserDto>())
            .Map(dest => dest.PostId, src => src.PostId)
            .Map(dest => dest.ParentCommentId, src => src.ParentCommentId)
            .Map(dest => dest.Replies, src => src.Replies.Adapt<ICollection<CommentDto>>());

        config.NewConfig<CreateCommentDto, Comment>()
            .Map(dest => dest.PostId, src => src.PostId)
            .Map(dest => dest.ParentCommentId, src => src.ParentCommentId)
            .Map(dest => dest.Content, src => src.Content)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CommenterId)
            .Ignore(dest => dest.Commenter)
            .Ignore(dest => dest.CommentDate)
            .Ignore(dest => dest.IsApproved)
            .Ignore(dest => dest.Replies)
            .Ignore(dest => dest.ParentCommentId);

        config.NewConfig<UpdateCommentDto, Comment>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.IsApproved, src => src.IsApproved)
            .Ignore(dest => dest.CommenterId)
            .Ignore(dest => dest.Commenter)
            .Ignore(dest => dest.PostId)
            .Ignore(dest => dest.ParentCommentId)
            .Ignore(dest => dest.CommentDate)
            .Ignore(dest => dest.Replies);

        // --- Notification Mappings ---

        config.NewConfig<Notification, NotificationDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Message, src => src.Message)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.IsRead, src => src.IsRead)
            .Map(dest => dest.SentDate, src => src.SentDate)
            .Map(dest => dest.RelatedEntityId, src => src.RelatedEntityId);
    }
}
