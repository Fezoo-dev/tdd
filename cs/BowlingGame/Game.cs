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
        public const int SPARE_BONUS = 5;

        public class Frame
        {
            public int score;
            public int countTries;

            public Frame() : this(0, 0) { }

            public Frame(int score, int countTries)
            {
                this.score = score;
                this.countTries = countTries;
            }

            public override bool Equals(object obj)
            {
                var frame = obj as Frame;
                if (frame == null)
                    return false;

                return score == frame.score && countTries == frame.countTries;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public int MaxFrameValue { get;}
        public int FramePinsAmount { get; }
        public int RollsPerFrame { get; }
        private int score;
        private Frame currentFrame;
        private bool isStrike;
        public List<Frame> Frames { get; }

        public Game(int maxFrameValue, int framePinsAmount, int rollsPerFrame)
        {
            RollsPerFrame = rollsPerFrame;
            FramePinsAmount = framePinsAmount;
            MaxFrameValue = maxFrameValue;
            currentFrame = new Frame();
            Frames = new List<Frame>();
        }

        private void AddNewFrame()
        {
            Frames.Add((currentFrame));
            currentFrame = new Frame();
            isStrike = false;
        }

        public void Roll(int pins)
        {
            if (pins < 0 || pins > FramePinsAmount)
                throw new ArgumentOutOfRangeException();
            
            if (Frames.Count >= MaxFrameValue)
                throw new IndexOutOfRangeException();
            
            currentFrame.countTries++;
            currentFrame.score += pins;
            if (isStrike)
            {
                Frames[Frames.Count - 1].score += pins;
                score += pins;
            }
                
            if (currentFrame.countTries == 1 && currentFrame.score == FramePinsAmount)
            {
                AddNewFrame();
                isStrike = true;
            }

            if (currentFrame.score > FramePinsAmount)
                throw new ArgumentOutOfRangeException();
            
            score += pins;
            if (currentFrame.countTries == RollsPerFrame && currentFrame.score == FramePinsAmount)
                score += SPARE_BONUS;

            if (currentFrame.countTries == RollsPerFrame)
            {
                AddNewFrame();
            }
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
            game = new Game(10, 10, 2);
        }

        private void Strike()
        {
            game.Roll(game.FramePinsAmount);
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
        public void GameShouldHas10Frames()
        {
            for(int i = 0; i < game.RollsPerFrame * game.MaxFrameValue; i++)
                game.Roll(1);
 
            Assert.AreEqual(game.MaxFrameValue, game.Frames.Count);
        }

        [Test]
        public void WhenCountFramesIsBiggerThan10_GameHasIndexOutOfRangeException()
        {
            for (var i = 0; i < game.RollsPerFrame * game.MaxFrameValue; i++)
                game.Roll(1);

            Action action = () => game.Roll(4);
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Test]
        public void WhenScoreIsBiggerTenOnOneRoll()
        {
            Action action = () => game.Roll(11);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void WhenScoreIsBiggerTenOnOneFrame()
        {
            game.Roll(9);
            Action action = () => game.Roll(9);
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void MaxPinsOnTwoRolls()
        {
            game.Roll(5);
            game.Roll(5);
            game.GetScore().Should().Be(15);
        }

        [Test]
        public void RollIsStrikeGaveStrikeBonus()
        {
            Strike();
            game.Roll(1);
            game.Roll(2);
            Assert.AreEqual(16, game.GetScore());
        }

        [Test]
        public void GetScore_OnNextFramesAfterStrike()
        {
            Strike();
            game.Roll(1);
            game.Roll(2);
            game.Roll(1);
            game.Roll(2);
            Assert.AreEqual(19, game.GetScore());
        }

        [Test]
        public void MaxScoreOnOneGame()
        {

        }
    }
}