//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kursach
{
    using System;
    using System.Collections.Generic;
    
    public partial class workouts
    {
        public int workout_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public string workout_type { get; set; }
        public Nullable<int> duration { get; set; }
        public Nullable<int> calories_burned { get; set; }
        public System.DateTime date { get; set; }
    
        public virtual users users { get; set; }
    }
}
