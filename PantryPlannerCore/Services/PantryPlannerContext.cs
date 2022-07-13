using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PantryPlannerCore.Models;

namespace PantryPlanner.Services
{
    public partial class PantryPlannerContext : IdentityDbContext<PantryPlannerUser>
    {
        public PantryPlannerContext()
        {
        }

        public PantryPlannerContext(DbContextOptions<PantryPlannerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryType> CategoryTypes { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<IngredientTag> IngredientTags { get; set; } = null!;
        public virtual DbSet<Kitchen> Kitchens { get; set; } = null!;
        public virtual DbSet<KitchenIngredient> KitchenIngredients { get; set; } = null!;
        public virtual DbSet<KitchenList> KitchenLists { get; set; } = null!;
        public virtual DbSet<KitchenListIngredient> KitchenListIngredients { get; set; } = null!;
        public virtual DbSet<KitchenRecipe> KitchenRecipes { get; set; } = null!;
        public virtual DbSet<KitchenUser> KitchenUsers { get; set; } = null!;
        public virtual DbSet<MealPlan> MealPlans { get; set; } = null!;
        public virtual DbSet<MealPlanRecipe> MealPlanRecipes { get; set; } = null!;
        public virtual DbSet<Recipe> Recipes { get; set; } = null!;
        public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;
        public virtual DbSet<RecipeStep> RecipeSteps { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "app");

                entity.HasIndex(e => e.CreatedByKitchenId, "IX_Category_CreatedByKitchenId");

                entity.HasIndex(e => e.CategoryTypeId, "fkIdx_161");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryTypeId).HasColumnName("CategoryTypeID");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CategoryType)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.CategoryTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("TypeToCategoryFK");

                entity.HasOne(d => d.CreatedByKitchen)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.CreatedByKitchenId)
                    .HasConstraintName("KitchenToCategoryFK");
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.ToTable("CategoryType", "app");

                entity.Property(e => e.CategoryTypeId).HasColumnName("CategoryTypeID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "app");

                entity.HasIndex(e => e.CategoryId, "IX_Ingredient_CategoryId");

                entity.HasIndex(e => e.AddedByUserId, "fkIdx_40");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.AddedByUserId).HasColumnName("AddedByUserID");

                entity.Property(e => e.DateAdded).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.PreviewPicture).HasColumnType("image");

                entity.HasOne(d => d.AddedByUser)
                    .WithMany(p => p.Ingredient)
                    .HasForeignKey(d => d.AddedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("UserToIngredientFK");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToIngredientFK");
            });

            modelBuilder.Entity<IngredientTag>(entity =>
            {
                entity.ToTable("IngredientTag", "app");

                entity.HasIndex(e => e.IngredientId, "fkIdx_198");

                entity.HasIndex(e => e.KitchenId, "fkIdx_201");

                entity.HasIndex(e => e.CreatedByKitchenUserId, "fkIdx_204");

                entity.Property(e => e.IngredientTagId).HasColumnName("IngredientTagID");

                entity.Property(e => e.CreatedByKitchenUserId).HasColumnName("CreatedByKitchenUserID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.TagName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByKitchenUser)
                    .WithMany(p => p.IngredientTags)
                    .HasForeignKey(d => d.CreatedByKitchenUserId)
                    .HasConstraintName("UserToTagFK");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.IngredientTags)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToTagFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.IngredientTags)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToTagFK");
            });

            modelBuilder.Entity<Kitchen>(entity =>
            {
                entity.ToTable("Kitchen", "app");

                entity.HasIndex(e => e.CreatedByUserId, "IX_Kitchen_CreatedByUserId");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Kitchen)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("UserToKitchenFK");
            });

            modelBuilder.Entity<KitchenIngredient>(entity =>
            {
                entity.HasKey(e => new { e.KitchenIngredientId, e.IngredientId, e.KitchenId });

                entity.ToTable("KitchenIngredient", "app");

                entity.HasIndex(e => e.AddedByKitchenUserId, "fkIdx_115");

                entity.HasIndex(e => e.CategoryId, "fkIdx_187");

                entity.HasIndex(e => e.KitchenId, "fkIdx_47");

                entity.HasIndex(e => e.IngredientId, "fkIdx_50");

                entity.Property(e => e.KitchenIngredientId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("KitchenIngredientID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.AddedByKitchenUserId).HasColumnName("AddedByKitchenUserID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(N'')");

                entity.HasOne(d => d.AddedByKitchenUser)
                    .WithMany(p => p.KitchenIngredients)
                    .HasForeignKey(d => d.AddedByKitchenUserId)
                    .HasConstraintName("KitchenUserToIngredientFK");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenIngredients)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToKitchenIngredientFK");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.KitchenIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToKitchenIngredientFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenIngredients)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenIngredientFK");
            });

            modelBuilder.Entity<KitchenList>(entity =>
            {
                entity.ToTable("KitchenList", "app");

                entity.HasIndex(e => e.KitchenId, "fkIdx_145");

                entity.HasIndex(e => e.CategoryId, "fkIdx_175");

                entity.Property(e => e.KitchenListId).HasColumnName("KitchenListID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenLists)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToListFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenLists)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToListFK");
            });

            modelBuilder.Entity<KitchenListIngredient>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.KitchenListId, e.IngredientId })
                    .HasName("PK_KitchenListRecipe");

                entity.ToTable("KitchenListIngredient", "app");

                entity.HasIndex(e => e.CategoryId, "IX_KitchenListIngredient_CategoryId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.KitchenListId).HasColumnName("KitchenListID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.HasOne(d => d.AddedFromRecipe)
                    .WithMany(p => p.KitchenListIngredients)
                    .HasForeignKey(d => d.AddedFromRecipeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("RecipeToListIngredientFK");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenListIngredients)
                    .HasForeignKey(d => d.CategoryId);

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.KitchenListIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToListIngredientFK");

                entity.HasOne(d => d.KitchenList)
                    .WithMany(p => p.KitchenListIngredients)
                    .HasForeignKey(d => d.KitchenListId)
                    .HasConstraintName("KitchenListToIngredientFK");
            });

            modelBuilder.Entity<KitchenRecipe>(entity =>
            {
                entity.HasKey(e => new { e.KitchenRecipeId, e.KitchenId, e.RecipeId });

                entity.ToTable("KitchenRecipe", "app");

                entity.HasIndex(e => e.RecipeId, "fkIdx_100");

                entity.HasIndex(e => e.CategoryId, "fkIdx_190");

                entity.HasIndex(e => e.KitchenId, "fkIdx_97");

                entity.Property(e => e.KitchenRecipeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("KitchenRecipeID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenRecipes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToKitchenRecipeFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenRecipes)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenRecipeFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.KitchenRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToKitchenRecipeFK");
            });

            modelBuilder.Entity<KitchenUser>(entity =>
            {
                entity.ToTable("KitchenUser", "app");

                entity.HasIndex(e => e.UserId, "fkIdx_19");

                entity.HasIndex(e => e.KitchenId, "fkIdx_22");

                entity.Property(e => e.KitchenUserId).HasColumnName("KitchenUserID");

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenUsers)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToKitchenUserFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.KitchenUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("UserToKitchenUserFK");
            });

            modelBuilder.Entity<MealPlan>(entity =>
            {
                entity.ToTable("MealPlan", "app");

                entity.HasIndex(e => e.KitchenId, "fkIdx_108");

                entity.HasIndex(e => e.CreatedByKitchenUserId, "fkIdx_123");

                entity.HasIndex(e => e.CategoryId, "fkIdx_181");

                entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreatedByKitchenUserId).HasColumnName("CreatedByKitchenUserID");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.KitchenId).HasColumnName("KitchenID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MealPlans)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("CategoryToMealPlanFK");

                entity.HasOne(d => d.CreatedByKitchenUser)
                    .WithMany(p => p.MealPlans)
                    .HasForeignKey(d => d.CreatedByKitchenUserId)
                    .HasConstraintName("KitchenUserToMealPlanFK");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.MealPlans)
                    .HasForeignKey(d => d.KitchenId)
                    .HasConstraintName("KitchenToMealPlanFK");
            });

            modelBuilder.Entity<MealPlanRecipe>(entity =>
            {
                entity.HasKey(e => new { e.MealPlanRecipeId, e.RecipeId, e.MealPlanId });

                entity.ToTable("MealPlanRecipe", "app");

                entity.HasIndex(e => e.RecipeId, "fkIdx_133");

                entity.HasIndex(e => e.MealPlanId, "fkIdx_136");

                entity.Property(e => e.MealPlanRecipeId).HasColumnName("MealPlanRecipeID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.MealPlanId).HasColumnName("MealPlanID");

                entity.HasOne(d => d.MealPlan)
                    .WithMany(p => p.MealPlanRecipes)
                    .HasForeignKey(d => d.MealPlanId)
                    .HasConstraintName("MealPlanToRecipeFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.MealPlanRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToMealPlanFK");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe", "app");

                entity.HasIndex(e => e.CreatedByUserId, "fkIdx_69");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.IsPublic)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.RecipeUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ServingSize).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("UserToRecipeFK");
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.HasKey(e => new { e.RecipeIngredientId, e.IngredientId, e.RecipeId });

                entity.ToTable("RecipeIngredient", "app");

                entity.Property(e => e.RecipeIngredientId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("RecipeIngredientID");

                entity.Property(e => e.IngredientId).HasColumnName("IngredientID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.Method)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnType("decimal(12, 4)");

                entity.Property(e => e.UnitOfMeasure)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.RecipeIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("IngredientToRecipeIngredientFK");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeIngredients)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToRecipeIngredientFK");
            });

            modelBuilder.Entity<RecipeStep>(entity =>
            {
                entity.HasKey(e => new { e.RecipeStepId, e.RecipeId });

                entity.ToTable("RecipeStep", "app");

                entity.Property(e => e.RecipeStepId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("RecipeStepID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.Text).HasMaxLength(500);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeSteps)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("RecipeToStepFK");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
