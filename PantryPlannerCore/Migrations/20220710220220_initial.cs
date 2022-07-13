using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PantryPlannerCore.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryType",
                schema: "app",
                columns: table => new
                {
                    CategoryTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryType", x => x.CategoryTypeID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kitchen",
                schema: "app",
                columns: table => new
                {
                    KitchenID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniquePublicGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchen", x => x.KitchenID);
                    table.ForeignKey(
                        name: "UserToKitchenFK",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                schema: "app",
                columns: table => new
                {
                    RecipeID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RecipeUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrepTime = table.Column<int>(type: "int", nullable: true),
                    CookTime = table.Column<int>(type: "int", nullable: true),
                    ServingSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.RecipeID);
                    table.ForeignKey(
                        name: "UserToRecipeFK",
                        column: x => x.CreatedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "app",
                columns: table => new
                {
                    CategoryID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryTypeID = table.Column<int>(type: "int", nullable: true),
                    CreatedByKitchenId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                    table.ForeignKey(
                        name: "KitchenToCategoryFK",
                        column: x => x.CreatedByKitchenId,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID");
                    table.ForeignKey(
                        name: "TypeToCategoryFK",
                        column: x => x.CategoryTypeID,
                        principalSchema: "app",
                        principalTable: "CategoryType",
                        principalColumn: "CategoryTypeID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KitchenUser",
                schema: "app",
                columns: table => new
                {
                    KitchenUserID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    HasAcceptedInvite = table.Column<bool>(type: "bit", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenUser", x => x.KitchenUserID);
                    table.ForeignKey(
                        name: "KitchenToKitchenUserFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UserToKitchenUserFK",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeStep",
                schema: "app",
                columns: table => new
                {
                    RecipeStepID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeID = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeStep", x => new { x.RecipeStepID, x.RecipeID });
                    table.ForeignKey(
                        name: "RecipeToStepFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                schema: "app",
                columns: table => new
                {
                    IngredientID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviewPicture = table.Column<byte[]>(type: "image", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.IngredientID);
                    table.ForeignKey(
                        name: "CategoryToIngredientFK",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "UserToIngredientFK",
                        column: x => x.AddedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KitchenList",
                schema: "app",
                columns: table => new
                {
                    KitchenListID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    CategoryID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenList", x => x.KitchenListID);
                    table.ForeignKey(
                        name: "CategoryToListFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToListFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenRecipe",
                schema: "app",
                columns: table => new
                {
                    KitchenRecipeID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    RecipeID = table.Column<long>(type: "bigint", nullable: false),
                    CategoryID = table.Column<long>(type: "bigint", nullable: true),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenRecipe", x => new { x.KitchenRecipeID, x.KitchenID, x.RecipeID });
                    table.ForeignKey(
                        name: "CategoryToKitchenRecipeFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToKitchenRecipeFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToKitchenRecipeFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlan",
                schema: "app",
                columns: table => new
                {
                    MealPlanID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByKitchenUserID = table.Column<long>(type: "bigint", nullable: true),
                    CategoryID = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlan", x => x.MealPlanID);
                    table.ForeignKey(
                        name: "CategoryToMealPlanFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "KitchenToMealPlanFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenUserToMealPlanFK",
                        column: x => x.CreatedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID");
                });

            migrationBuilder.CreateTable(
                name: "IngredientTag",
                schema: "app",
                columns: table => new
                {
                    IngredientTagID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientID = table.Column<long>(type: "bigint", nullable: false),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByKitchenUserID = table.Column<long>(type: "bigint", nullable: true),
                    TagName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTag", x => x.IngredientTagID);
                    table.ForeignKey(
                        name: "IngredientToTagFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenToTagFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UserToTagFK",
                        column: x => x.CreatedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID");
                });

            migrationBuilder.CreateTable(
                name: "KitchenIngredient",
                schema: "app",
                columns: table => new
                {
                    KitchenIngredientID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientID = table.Column<long>(type: "bigint", nullable: false),
                    KitchenID = table.Column<long>(type: "bigint", nullable: false),
                    AddedByKitchenUserID = table.Column<long>(type: "bigint", nullable: true),
                    CategoryID = table.Column<long>(type: "bigint", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getutcdate())"),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, defaultValueSql: "(N'')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenIngredient", x => new { x.KitchenIngredientID, x.IngredientID, x.KitchenID });
                    table.ForeignKey(
                        name: "CategoryToKitchenIngredientFK",
                        column: x => x.CategoryID,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "IngredientToKitchenIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenToKitchenIngredientFK",
                        column: x => x.KitchenID,
                        principalSchema: "app",
                        principalTable: "Kitchen",
                        principalColumn: "KitchenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenUserToIngredientFK",
                        column: x => x.AddedByKitchenUserID,
                        principalSchema: "app",
                        principalTable: "KitchenUser",
                        principalColumn: "KitchenUserID");
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredient",
                schema: "app",
                columns: table => new
                {
                    RecipeIngredientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientID = table.Column<long>(type: "bigint", nullable: false),
                    RecipeID = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(12,4)", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Method = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredient", x => new { x.RecipeIngredientID, x.IngredientID, x.RecipeID });
                    table.ForeignKey(
                        name: "IngredientToRecipeIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToRecipeIngredientFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenListIngredient",
                schema: "app",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KitchenListID = table.Column<long>(type: "bigint", nullable: false),
                    IngredientID = table.Column<long>(type: "bigint", nullable: false),
                    AddedFromRecipeId = table.Column<long>(type: "bigint", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsChecked = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenListRecipe", x => new { x.ID, x.KitchenListID, x.IngredientID });
                    table.ForeignKey(
                        name: "FK_KitchenListIngredient_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "app",
                        principalTable: "Category",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "IngredientToListIngredientFK",
                        column: x => x.IngredientID,
                        principalSchema: "app",
                        principalTable: "Ingredient",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "KitchenListToIngredientFK",
                        column: x => x.KitchenListID,
                        principalSchema: "app",
                        principalTable: "KitchenList",
                        principalColumn: "KitchenListID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToListIngredientFK",
                        column: x => x.AddedFromRecipeId,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanRecipe",
                schema: "app",
                columns: table => new
                {
                    MealPlanRecipeID = table.Column<int>(type: "int", nullable: false),
                    RecipeID = table.Column<long>(type: "bigint", nullable: false),
                    MealPlanID = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanRecipe", x => new { x.MealPlanRecipeID, x.RecipeID, x.MealPlanID });
                    table.ForeignKey(
                        name: "MealPlanToRecipeFK",
                        column: x => x.MealPlanID,
                        principalSchema: "app",
                        principalTable: "MealPlan",
                        principalColumn: "MealPlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RecipeToMealPlanFK",
                        column: x => x.RecipeID,
                        principalSchema: "app",
                        principalTable: "Recipe",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "fkIdx_161",
                schema: "app",
                table: "Category",
                column: "CategoryTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedByKitchenId",
                schema: "app",
                table: "Category",
                column: "CreatedByKitchenId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_40",
                schema: "app",
                table: "Ingredient",
                column: "AddedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_CategoryId",
                schema: "app",
                table: "Ingredient",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_198",
                schema: "app",
                table: "IngredientTag",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_201",
                schema: "app",
                table: "IngredientTag",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_204",
                schema: "app",
                table: "IngredientTag",
                column: "CreatedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_CreatedByUserId",
                schema: "app",
                table: "Kitchen",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "fkIdx_115",
                schema: "app",
                table: "KitchenIngredient",
                column: "AddedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_187",
                schema: "app",
                table: "KitchenIngredient",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_47",
                schema: "app",
                table: "KitchenIngredient",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_50",
                schema: "app",
                table: "KitchenIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_145",
                schema: "app",
                table: "KitchenList",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_175",
                schema: "app",
                table: "KitchenList",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_AddedFromRecipeId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "AddedFromRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_CategoryId",
                schema: "app",
                table: "KitchenListIngredient",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_IngredientID",
                schema: "app",
                table: "KitchenListIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenListIngredient_KitchenListID",
                schema: "app",
                table: "KitchenListIngredient",
                column: "KitchenListID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_100",
                schema: "app",
                table: "KitchenRecipe",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_190",
                schema: "app",
                table: "KitchenRecipe",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_97",
                schema: "app",
                table: "KitchenRecipe",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_19",
                schema: "app",
                table: "KitchenUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_22",
                schema: "app",
                table: "KitchenUser",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_108",
                schema: "app",
                table: "MealPlan",
                column: "KitchenID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_123",
                schema: "app",
                table: "MealPlan",
                column: "CreatedByKitchenUserID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_181",
                schema: "app",
                table: "MealPlan",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_133",
                schema: "app",
                table: "MealPlanRecipe",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_136",
                schema: "app",
                table: "MealPlanRecipe",
                column: "MealPlanID");

            migrationBuilder.CreateIndex(
                name: "fkIdx_69",
                schema: "app",
                table: "Recipe",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredient_IngredientID",
                schema: "app",
                table: "RecipeIngredient",
                column: "IngredientID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredient_RecipeID",
                schema: "app",
                table: "RecipeIngredient",
                column: "RecipeID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeStep_RecipeID",
                schema: "app",
                table: "RecipeStep",
                column: "RecipeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "IngredientTag",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenListIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenRecipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MealPlanRecipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RecipeIngredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RecipeStep",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "KitchenList",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MealPlan",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Ingredient",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Recipe",
                schema: "app");

            migrationBuilder.DropTable(
                name: "KitchenUser",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Kitchen",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CategoryType",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
