using System;
using System.Collections.Generic;
using System.Linq;
using BowlingGame.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace BowlingGame
{
    public class Game
    {
        public class Frame
        {
            public int score;
            public int CountTries;

            public Frame(int score, int countTries)
            {
                this.score = score;
                this.CountTries = countTries;
            }

            public override bool Equals(object obj)
            {
                var frame = obj as Frame;
                if (frame == null)
                    return false;

                return score == frame.score && CountTries == frame.CountTries;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private int score;
        private Frame currentFrame;

        public List<Frame> Frames { get; private set; }

        public Game()
        {
            score = 0;
            currentFrame = new Frame(0, 0);
            Frames = new List<Frame>();
        }

        public void Roll(int pins)
        {

            currentFrame.CountTries++;
            currentFrame.score += pins;

            if (currentFrame.CountTries == 2)
            {
                Frames.Add(currentFrame);
                currentFrame = new Frame(0,0);
            }

            score += pins;
        }

        public int GetScore()
        {
            return score;
        }
    }

    [TestFixture]
    public class Game_should : ReportingTest<Game_should>
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            game = new Game();
        }

        [Test]
        public void HaveZeroScore_BeforeAnyRolls()
        {
            game
                .GetScore()
                .Should().Be(0);
        }

        [Test]
        public void HaveNScore_WhenRollNPins()
        {
            game.Roll(5);
            game.GetScore().Should().Be(5);
        }

            
        [Test]
        public void HaveTwoTriesOnFrame()
        {
            game.Roll(3);
            game.Roll(4);
            var res = game.Frames.Contains(new Game.Frame(7, 2));
            res.Should().BeTrue();
        }

        [Test]
        public void GameHas10Frames()
        {
            for(int i = 0; i<20; i++)
                game.Roll(i);
 
            Assert.AreEqual(10, game.Frames.Count);
        }

        [Test]
        public void WhenCountFramesIsBiggerThan10_GameHasIndexOutOfRangeException()
        {
            for (var i = 0; i < 20; i++)
                game.Roll(i);

            Action action = () => game.Roll(4);
            action.Should().Throw<IndexOutOfRangeException>();
        }
    }
}