﻿// <auto-generated />
using System;
using IdeaTree.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdeaTree.Migrations
{
    [DbContext(typeof(IdeaTreeContext))]
    partial class IdeaTreeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdeaTree.Models.Comment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int?>("PostedByID");

                    b.Property<int?>("PostedToID");

                    b.Property<int>("Status");

                    b.HasKey("ID");

                    b.HasIndex("PostedByID");

                    b.HasIndex("PostedToID");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("IdeaTree.Models.Idea", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int?>("ModifiedByID");

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<DateTime>("PostDate");

                    b.Property<int?>("PostedByID");

                    b.Property<int>("Status");

                    b.Property<string>("Tags");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.HasKey("ID");

                    b.HasIndex("ModifiedByID");

                    b.HasIndex("PostedByID");

                    b.ToTable("Idea");
                });

            modelBuilder.Entity("IdeaTree.Models.Member", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Bio");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Email");

                    b.Property<string>("FullName")
                        .HasMaxLength(150);

                    b.Property<string>("Image");

                    b.Property<DateTime?>("LastLogon");

                    b.Property<int>("MType");

                    b.Property<int?>("ModifiedByID");

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<bool>("Newsletter");

                    b.Property<string>("Password");

                    b.Property<DateTime?>("PasswordGenerateDate");

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.Property<int>("Status");

                    b.HasKey("ID");

                    b.HasIndex("ModifiedByID");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("IdeaTree.Models.Message", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BCC");

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<string>("CC");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("FromEmail");

                    b.Property<string>("FromName")
                        .HasMaxLength(100);

                    b.Property<DateTime>("SentDate");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ToEmail");

                    b.Property<string>("ToName")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("IdeaTree.Models.Vote", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<int?>("VoteToID");

                    b.Property<int?>("VotedByID");

                    b.HasKey("ID");

                    b.HasIndex("VoteToID");

                    b.HasIndex("VotedByID");

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("IdeaTree.Models.Comment", b =>
                {
                    b.HasOne("IdeaTree.Models.Member", "PostedBy")
                        .WithMany()
                        .HasForeignKey("PostedByID");

                    b.HasOne("IdeaTree.Models.Idea", "PostedTo")
                        .WithMany()
                        .HasForeignKey("PostedToID");
                });

            modelBuilder.Entity("IdeaTree.Models.Idea", b =>
                {
                    b.HasOne("IdeaTree.Models.Member", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByID");

                    b.HasOne("IdeaTree.Models.Member", "PostedBy")
                        .WithMany()
                        .HasForeignKey("PostedByID");
                });

            modelBuilder.Entity("IdeaTree.Models.Member", b =>
                {
                    b.HasOne("IdeaTree.Models.Member", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedByID");
                });

            modelBuilder.Entity("IdeaTree.Models.Vote", b =>
                {
                    b.HasOne("IdeaTree.Models.Idea", "VoteTo")
                        .WithMany()
                        .HasForeignKey("VoteToID");

                    b.HasOne("IdeaTree.Models.Member", "VotedBy")
                        .WithMany()
                        .HasForeignKey("VotedByID");
                });
#pragma warning restore 612, 618
        }
    }
}
