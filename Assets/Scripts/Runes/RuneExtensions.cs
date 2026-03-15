namespace Runes
{
    public static class RuneExtensions
    {
        /// <summary>
        /// Содержит ли группа сущность
        /// </summary>
        /// <param name="group">Группа, в которой ищется сущность</param>
        /// <param name="type">Сущность, которая должна быть в группе</param>
        /// <returns>Есть ли сущность в группе</returns>
        public static bool Contains(this RuneGroup group, RuneType type)
        {
            var groupValue = (int)group;
            var value = (int)type;
            return (groupValue & value) == value;
        }
        
        /// <summary>
        /// Добавляет в группу сущность, аналогичен And
        /// </summary>
        /// <param name="group">Группа, в которую добавляют сущность</param>
        /// <param name="type">Сущность, которую добавляют в группу</param>
        /// <returns>Новая группа с уже добавленной сущностью</returns>
        public static RuneGroup Add(this RuneGroup group, RuneType type)
        {
            var groupValue = (int)group;
            var value = (int)type;
            groupValue |= value;
            return (RuneGroup)groupValue;
        }
        /// <summary>
        /// Удаляет из группы сущность
        /// </summary>
        /// <param name="group">Группа, из которой удаляют сущность</param>
        /// <param name="type">Сущность, которую удаляют из группы</param>
        /// <returns>Новую группу уже без удалённой сущности</returns>
        public static RuneGroup Remove(this RuneGroup group, RuneType type)
        {
            var groupValue = (int)group;
            var value = (int)type;
            groupValue &= ~value;
            return (RuneGroup)groupValue;
        }
        
        /// <summary>
        /// Применяет операцию И для группы и сущности, аналогичен Add
        /// </summary>
        /// <returns>Группу, которая либо пустая, либо содержащая только type</returns>
        public static RuneGroup And(this RuneGroup group, RuneType type) // Any
        {
            var groupValue = (int)group;
            var value = (int)type;
            groupValue &= value;
            return (RuneGroup)groupValue;
        }
        /// <summary>
        /// Применяет операцию ИЛИ для группы и сущности, аналогичен Add
        /// </summary>
        /// <returns>Новая группа с уже добавленной сущностью</returns>
        public static RuneGroup Or(this RuneGroup group, RuneType type) // All
        {
            var groupValue = (int)group;
            var value = (int)type;
            groupValue |= value;
            return (RuneGroup)groupValue;
        }
        
        /// <summary>
        /// Равны ли группы по содержанию?
        /// </summary>
        public static bool Equals(this RuneGroup group, RuneGroup otherGroup)
        {
            var groupValue = (int)group;
            var groupValue2 = (int)otherGroup;
            return groupValue == groupValue2;
        }
        
        /// <summary>
        /// Добавляет в группу другую группу, аналогичен And
        /// </summary>
        /// <param name="group">Группа, в которую добавляют другую группу</param>
        /// <param name="otherGroup">Другая группа, которая добавляется в группу</param>
        /// <returns>Новая группа с уже добавленной другой группой</returns>
        public static RuneGroup Add(this RuneGroup group, RuneGroup otherGroup)
        {
            var groupValue = (int)group;
            var value = (int)otherGroup;
            groupValue |= value;
            return (RuneGroup)groupValue;
        }
        /// <summary>
        /// Удаляет из группы другую группу
        /// </summary>
        /// <param name="group">Группа, из которой удаляют другую группу</param>
        /// <param name="otherGroup">Другая группа, которую удаляют из группы</param>
        /// <returns>Новую группу, которая уже не содержит элементы из другой группы</returns>
        public static RuneGroup Remove(this RuneGroup group, RuneGroup otherGroup)
        {
            var groupValue = (int)group;
            var value = (int)otherGroup;
            groupValue &= ~value;
            return (RuneGroup)groupValue;
        }
        
        /// <summary>
        /// Применяет операцию И для 2 групп, аналогичен Add
        /// </summary>
        /// <returns>Группу, которая содержит элементы, содержащиеся только в обоих группах</returns>
        public static RuneGroup And(this RuneGroup group, RuneGroup otherGroup) // Any
        {
            return group & otherGroup;
        }
        /// <summary>
        /// Применяет операцию ИЛИ для 2 групп
        /// </summary>
        /// <returns>Группу, которая содержит элементы, содержащиеся хотя-бы в одной из групп</returns>
        public static RuneGroup Or(this RuneGroup group, RuneGroup otherGroup) // All
        {
            return group | otherGroup;
        }
    }
}