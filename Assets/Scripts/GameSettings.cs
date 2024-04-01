using System;
using System.Collections;
using System.Collections.Generic;
using Expressions.Reference;
using UnityEngine;
using DEFINE_TOKEN = Expressions.Reference.Identifier.Defined;

// this shit makes javascript look reasonable.
public class GameSettings
{
    public delegate Expressions.Expression<IEnumerable<GameActions.IResolution>> ImplementAbility(Ability.IAbility ability);

    public ImplementAbility AbilityImplementation;

    public GameSettings (
         ImplementAbility ABILITY_IMPLEMENTATION
        )
    {
        AbilityImplementation = ABILITY_IMPLEMENTATION;
    }

    //placeholder constant
    public static GameSettings STANDARD = new(
        // The way the gun just appeared in my mouth.
        ABILITY_IMPLEMENTATION: (ability) =>
        {
            return ability switch
            {
                // hypothetically ~~ referring to another definition within a definition should be ok, it *should* evaluate when needed.
                Ability.Attack.Data attack => new(
                    new(
                        (new DEFINE_TOKEN(typeof(Ability.Attack.DefinedTokens.Source)), Referable.Create(new Tokens.Gen.Select.One<Unit>(new Tokens.GameData.AllUnits()))),
                        (new DEFINE_TOKEN(typeof(Ability.Attack.DefinedTokens.Target)), Referable.Create(new Ability.Attack.DefinedTokens.Source()))
                    ),
                    attack.ActionToken
                )
            };
        }
        );
}
