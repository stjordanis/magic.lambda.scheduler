﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019, thomas@gaiasoul.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using magic.node;

namespace magic.lambda.scheduler.utilities.jobs
{
    /// <summary>
    /// Class wrapping a single job, with its repetition pattern being "every n'th of x",
    /// where n is a positive integer value, and x is some sort of entity, such as "days", "hours", etc -
    /// And its associated lambda object to be executed when the job is due.
    /// </summary>
    public class EveryEntityRepeatJob : RepeatJob
    {
        /// <summary>
        /// Repetition pattern for job.
        /// </summary>
        public enum RepetitionEntityType
        {
            /// <summary>
            /// Every n second.
            /// </summary>
            seconds,

            /// <summary>
            /// Every n minute.
            /// </summary>
            minutes,

            /// <summary>
            /// Every n hour.
            /// </summary>
            hours,

            /// <summary>
            /// Every n day.
            /// </summary>
            days
        };

        readonly private RepetitionEntityType _repetition;
        readonly private long _repetitionValue;

        /// <summary>
        /// Creates a new job that repeat every n days/hours/minutes/seconds.
        /// </summary>
        /// <param name="name">Name of new job.</param>
        /// <param name="description">Description of job.</param>
        /// <param name="lambda">Lambda object to execute as job is due.</param>
        /// <param name="repetition">Repetition pattern.</param>
        /// <param name="repetitionValue">Number of entities declared through repetition pattern.</param>
        public EveryEntityRepeatJob(
            string name, 
            string description, 
            Node lambda,
            RepetitionEntityType repetition,
            long repetitionValue)
            : base(name, description, lambda)
        {
            // Sanity checking and decorating instance.
            if (repetitionValue < 1)
                throw new ArgumentException("Repetition value must be a positive integer value.");

            _repetition = repetition;
            _repetitionValue = repetitionValue;
        }

        /// <summary>
        /// Returns the node representation of the job.
        /// </summary>
        /// <returns>A node representing the declaration of the job as when created.</returns>
        public override Node GetNode()
        {
            var result = new Node(Name);
            if (!string.IsNullOrEmpty(Description))
                result.Add(new Node("description", Description));
            result.Add(new Node("repeat", _repetition.ToString(), new Node[] { new Node("value", _repetitionValue) }));
            result.Add(new Node(".lambda", null, Lambda.Children.Select(x => x.Clone())));
            return result;
        }

        #region [ -- Overridden abstract base class methods -- ]

        /// <summary>
        /// Calculates the job's next due date.
        /// </summary>
        protected override void CalculateNextDue()
        {
            switch (_repetition)
            {
                case RepetitionEntityType.seconds:
                    Due = DateTime.Now.AddSeconds(_repetitionValue);
                    break;

                case RepetitionEntityType.minutes:
                    Due = DateTime.Now.AddMinutes(_repetitionValue);
                    break;

                case RepetitionEntityType.hours:
                    Due = DateTime.Now.AddHours(_repetitionValue);
                    break;

                case RepetitionEntityType.days:
                    Due = DateTime.Now.AddDays(_repetitionValue);
                    break;

                default:
                    throw new ApplicationException("Oops, you've made it into an impossible code branch!");
            }
        }

        #endregion
    }
}
