﻿using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BlazingBlog.Models
{
    public class BlogPostSaveModel
    {
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string Title { get; set; }
        [Required, MaxLength(150)]
        public string Slug { get; set; }
        public int CategoryId { get; set; }
        [Required, MaxLength(250)]
        public string Introduction { get; set; }
        public string? Content { get; set; }
        public bool IsPublished { get; set; }
        public BlogPost ToBlogPostEntity(int userId) =>
            new() {
            Id = Id,
            Title = Title,
            Slug = Slug,
            CategoryId = CategoryId,
            Introduction = Introduction,
            Content = Content,
            IsPublished = IsPublished,
            UserId = userId
            };
        public BlogPost Merge(BlogPost entity)
        {
            entity.Title = Title;
            entity.Slug = Slug;
            entity.CategoryId = CategoryId;
            entity.Introduction = Introduction;
            entity.Content = Content;
            entity.IsPublished = IsPublished;
            return entity;
        }
        public static Expression<Func<BlogPost, BlogPostSaveModel>> Selector =>
           bp => new BlogPostSaveModel
           {
               Id = bp.Id,
               Title = bp.Title,
               Slug = bp.Slug,
               CategoryId = bp.CategoryId,
               Introduction = bp.Introduction,
               Content = bp.Content,
               IsPublished = bp.IsPublished
           };
    }
}
