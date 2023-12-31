﻿using Scriban.Runtime;
using Scriban;
using System;
using System.Collections.Generic;
using System.Text;
using SimpleDto.Generator.Templates.Abstractions;

namespace SimpleDto.Generator.Common;
internal sealed class ScribanRenderer
{
    public static string Render(BaseTemplate template)
    {
        var scriptObject = new ScriptObject();
        scriptObject.Import(template);

        var templateContext = new TemplateContext();
        templateContext.PushGlobal(scriptObject);

        var scribanTemplate = Template.Parse(template.GetTemplate());

        return scribanTemplate.Render(templateContext);
    }
}
