
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tasinmaz.DataAccess;

#nullable disable

namespace Tasinmaz.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20251110115515_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tasinmaz.Entities.Conrete.Il", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Iller");
                });

            modelBuilder.Entity("Tasinmaz.Entities.Conrete.Ilce", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("IlId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IlId");

                    b.ToTable("Ilceler");
                });

            modelBuilder.Entity("Tasinmaz.Entities.Conrete.Ilce", b =>
                {
                    b.HasOne("Tasinmaz.Entities.Conrete.Il", "Il")
                        .WithMany()
                        .HasForeignKey("IlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Il");
                });
#pragma warning restore 612, 618
        }
    }
}
