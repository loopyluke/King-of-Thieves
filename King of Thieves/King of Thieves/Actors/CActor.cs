﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using King_of_Thieves.Graphics;

namespace King_of_Thieves.Actors
{
    enum ACTORTYPES
    {
        MANAGER = 0,
        INTERACTABLE
    }

    abstract class CActor
    {
        protected Vector2 _position = Vector2.Zero;
        protected Vector2 _oldPosition = Vector2.Zero;
        public readonly ACTORTYPES ACTORTYPE;
        private string _name;
        protected List<Type> _collidables;
        protected CAnimation _sprite;
        
        public Graphics.CSprite image;
        //hitboxes will go here as well? What a terrible night for a curse...
        //event handlers will be added here

        public event createHandler onCreate;
        public event destroyHandler onDestroy;
        public event keyDownHandler onKeyDown;
        public event frameHandler onFrame;
        public event drawHandler onDraw;
        public event keyReleaseHandler onKeyRelease;
        public event collideHandler onCollide;
        public event animationEndHandler onAnimationEnd;

        public abstract void create(object sender);
        public abstract void destroy(object sender);
        public abstract void keyDown(object sender);
        public abstract void keyRelease(object sender);
        public abstract void frame(object sender);
        public abstract void draw(object sender);
        public abstract void collide(object sender, object collider);
        public abstract void animationEnd(object sender);

        protected abstract void _addCollidables(); //Use this guy to tell the Actor what kind of actors it can collide with

        public CAnimation spriteIndex
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
            }
        }

        public CActor(string name, ACTORTYPES type = ACTORTYPES.INTERACTABLE)
            
        {
            onCreate += new createHandler(create);
            onDestroy += new destroyHandler(destroy);
            onKeyDown += new keyDownHandler(keyDown);
            onKeyRelease += new keyReleaseHandler(keyRelease);
            onFrame += new frameHandler(frame);
            onDraw += new drawHandler(draw);
            onAnimationEnd += new animationEndHandler(animationEnd);

            ACTORTYPE = type;
            _name = name;
            _collidables = new List<Type>();

            try
            {
                _addCollidables();
            }
            catch (NotImplementedException)
            { ;}
               
            onCreate(this);
        }

        ~CActor()
        {
            onCreate -= new createHandler(create);
            onDestroy -= new destroyHandler(destroy);
            onKeyDown -= new keyDownHandler(keyDown);
            onFrame -= new frameHandler(frame);
            onKeyRelease -= new keyReleaseHandler(keyRelease);
            onDraw -= new drawHandler(draw);
        }

        public virtual void update(GameTime gameTime)
        {
            
            onFrame(this);

            _oldPosition = _position;
            image.X = (int)_position.X;
            image.Y = (int)_position.Y;

            if (Input.CInput.areKeysPressed)
                onKeyDown(this);

            if (Input.CInput.areKeysReleased)
                onKeyRelease(this);


        }

        public virtual void drawMe()
        {
            onDraw(this);

            image.draw();
        }

        public Vector2 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Vector2 oldPosition
        {
            get
            {
                return _oldPosition;
            }
            
        }

        public Vector2 distanceFromLastFrame
        {
            get
            {
                return (position - oldPosition);
            }
        }

        public virtual void remove()
        {
            onDestroy(this);
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        private void checkCollisions()
        {
            //This shit is WEIRD.
            //fetch my hitboxes
            List<BoundingBox> myBoxes = CMasterControl.hitboxes[this.GetType()][_name];
            
           
        }
    }
}
