using System;
using System.Collections.Generic;
using RetroToFileExporter.Core.Objects;
using RSDU.Domain;

namespace RetroToFileExporter.Core.Interfaces
{
    public interface IAction
    {
        /// <summary>
        /// Наименование триггера для отображения в Excel
        /// </summary>
        string NameTrigger { get; set; }
        
        /// <summary>
        /// Запрашиваемый ID кадра ретроспективы
        /// </summary>
        int KadrId { get; set; }
        
        /// <summary>
        /// Условия запроса
        /// </summary>
        List<QueryCondition> QueryConditions { get; set; }

        /// <summary>
        /// Добавления или исключения листа «Инфо» из результирующего файла
        /// </summary>
        bool Info { get; set; }

        /// <summary>
        /// Записываемые в лист данных поле Объект
        /// </summary>
        bool ObjectField { get; set; }
        
        /// <summary>
        /// Записываемые в лист данных поле Наименование типа параметра
        /// </summary>
        bool TypeParam { get; set; }

        /// <summary>
        /// Записываемые в лист данных поле Идентификатор параметра
        /// </summary>
        bool IdParam { get; set; }

        /// <summary>
        /// Записываемые в лист данных поле Идентификатор таблицы параметра 
        /// </summary>
        bool IdTableParam { get; set; }

        /// <summary>
        /// Записываемые в лист данных поле Наименование архивной таблицы 
        /// </summary>
        bool NameArchTable { get; set; }

        /// <summary>
        /// Записываемые в лист данных поле Статус значения (колонка)
        /// </summary>
        bool StateColumnHeader { get; set; }

        /// <summary>
        /// Путь до каталога размещения файлов
        /// </summary>
        string FilesFolder { get; set; }

        /// <summary>
        /// Имя файла данных 
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Тэги должны определяться относительно времени срабатывания триггера
        /// или первой/конечной даты из рассчитанных для запроса данных
        /// </summary>
        string TagTime { get; set; }

        /// <summary>
        /// Высокий уровень надежности 
        /// </summary>
        bool IsHighLevelReliability { get; set; }

        /// <summary>
        /// Запись в форматах Excel 2003 и 2010 (xls и xslx).
        /// </summary>
        string VersionExcel { get; set; }

        /// <summary>
        /// GUID триггера для записи в бд
        /// </summary>
        Guid GuidAction { get; set; }

        /// <summary>
        /// Сериализация
        /// </summary>
        /// <returns></returns>
        string ToXml();

        /// <summary>
        /// Смещение времени начала выполнения задачи (в секундах)
        /// </summary>
        int RunTimeOffset { get; set; }
    }
}