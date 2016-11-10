using System.Collections.Generic;
using AllReady.Models;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.ModelTests
{
    public class SkillShould
    {
        [Fact]
        public void HierarchicalName_ShouldReturnInvalidHierarchy_WhenAChildParentLoopIsDetected()
        {
            // DESCRIPTION: It's possible to get an infinite loop with the data so we need to ensure we short circuit that when evaluating the hierarchical name
            // NOTE: by sgordon - If the code we're testing still creates a loop this will cause this test to also run infinitely - not ideal!

            // Arrange
            var parentSkill = new Skill {Name = "Parent", Id = 1 };
            var childSkill = new Skill {Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };
            parentSkill.ParentSkill = childSkill;

            // Act

            var parentResult = parentSkill.HierarchicalName;
            var childResult = childSkill.HierarchicalName;

            // Assert
            parentResult.ShouldBe(Skill.InvalidHierarchy);
            childResult.ShouldBe(Skill.InvalidHierarchy);
        }

        [Fact]
        public void HierarchicalName_ReturnsCorrectName_WhenSkillHasNoChildren()
        {
            // Arrange
            var parentSkill = new Skill { Name = "Parent", Id = 1 };

            // Act

            var result = parentSkill.HierarchicalName;

            // Assert
            result.ShouldBe("Parent");
        }

        [Fact]
        public void HierarchicalName_ReturnsCorrectNameForParent_WhenSkillHasChildren()
        {
            // Arrange
            var parentSkill = new Skill { Name = "Parent", Id = 1 };
            var childSkill = new Skill { Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };

            // Act

            var result = parentSkill.HierarchicalName;

            // Assert
            result.ShouldBe("Parent");
        }

        [Fact]
        public void HierarchicalName_ReturnsCorrectNameForAChildSkill()
        {
            // Arrange
            var parentSkill = new Skill { Name = "Parent", Id = 1 };
            var childSkill = new Skill { Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };

            // Act

            var result = childSkill.HierarchicalName;

            // Assert
            result.ShouldBe("Parent > Child Skill");
        }

        [Fact]
        public void HierarchicalName_ReturnsCorrectNameForAThirdLevelAncestor()
        {
            // Arrange
            var parentSkill = new Skill { Name = "Parent", Id = 1 };
            var childSkill = new Skill { Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };
            var thridAncestorSkills = new Skill { Name = "Final Skill", ParentSkill = childSkill, Id = 3, ParentSkillId = 2 };

            // Act

            var result = thridAncestorSkills.HierarchicalName;

            // Assert
            result.ShouldBe("Parent > Child Skill > Final Skill");
        }

        [Fact]
        public void DescendantIds_ReturnsNull_WhenChildSkillsIsNull()
        {
            // Arrange
            var sut = new Skill { Id = 1, Name = "Skill" };

            // Act
            var result = sut.DescendantIds;

            result.ShouldBeNull();
        }

        [Fact]
        public void DescendantIds_ReturnsEmptyList_WhenChildSkillsIsEmpty()
        {
            // Arrange
            var sut = new Skill { Id = 1, Name = "Skill", ChildSkills = new List<Skill>()};

            // Act
            var result = sut.DescendantIds;

            result.ShouldBeEmpty();
        }

        [Fact]
        public void DescendantIds_ReturnsCorrectId_WhenSingleChild()
        {
            // Arrange
            var sut = new Skill { Id = 1, Name = "Skill", ChildSkills = new List<Skill> { new Skill() { Id = 2, Name = "Child" } } };

            // Act
            var result = sut.DescendantIds;

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(1);
            result.Contains(2).ShouldBeTrue();
        }

        [Fact]
        public void DescendantIds_ReturnsCorrectIds_WhenTwoChild_AtSameLevel()
        {
            // Arrange
            var sut = new Skill { Id = 1, Name = "Skill", ChildSkills = new List<Skill> { new Skill { Id = 2, Name = "Child" }, new Skill { Id = 3, Name= "Child 2"} } };

            // Act
            var result = sut.DescendantIds;

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(2);
            result.Contains(2).ShouldBeTrue();
            result.Contains(3).ShouldBeTrue();
        }

        [Fact]
        public void DescendantIds_ReturnsCorrectIds_WhenChildrenATMultipleLevels()
        {
            // Arrange
            var sut = new Skill {
                Id = 1,
                Name = "Skill",
                ChildSkills = new List<Skill> { new Skill {
                    Id = 2,
                    Name = "Child",
                    ChildSkills = new List<Skill> { new Skill {
                        Id = 3,
                        Name = "Child 2",
                        ChildSkills = new List<Skill> { new Skill
                            {
                                Id = 4,
                                Name = "Child 3"
                            }
                        }
                    }
                }}}};

            // Act
            var result = sut.DescendantIds;

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(3);
            result.Contains(2).ShouldBeTrue();
            result.Contains(3).ShouldBeTrue();
            result.Contains(4).ShouldBeTrue();
        }

        [Fact]
        public void DescendantIds_ReturnsNull_WhenThereIsALoopInTheHierarchy()
        {
            // Arrange
            var parentSkill = new Skill { Name = "Parent", Id = 1 };
            var childSkill = new Skill { Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };
            parentSkill.ParentSkill = childSkill;

            // Act
            var result = parentSkill.DescendantIds;

            result.ShouldBeNull();
        }
    }
}
